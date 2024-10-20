using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GBMU.Core;

public partial class PPU {
    public enum State {
        OAMSearch,
        PixelTransfer,
        HBlank,
        VBlank,
    }

    private readonly Memory _memory;
    private State _state;

    private readonly OAMEntry[] _oamEntries;

    // This is faster than using a List<T>
    private readonly OAMEntry[] _selectedOamEntries;
    private int _selectedOamEntriesCount = 0;

    private int _stateCycleCounter;
    private double _elapsedTime;

    private int _winLineCounter;
    private bool _wasDisplayEnabled;
    private readonly PPUScreens _screens;

    public PPU(Memory memory) {
        _memory = memory;

        _state = State.VBlank;
        _stateCycleCounter = 0;
        _winLineCounter = 0;

        _oamEntries = new OAMEntry[40];
        _selectedOamEntries = new OAMEntry[10];
        _selectedOamEntriesCount = 0;
        _screens = new PPUScreens(ScreenWidth, ScreenHeight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LCDC GetCurrentLCDC() => new(_memory.InternalReadByte(Memory.LCDC.Address));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LCDStatus GetCurrentLCDStatus() => new(_memory.InternalReadByte(Memory.STAT.Address));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint[] GetDisplayScreen() => _screens.GetDisplayScreen();

    public void Update(double deltaTime) {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= CycleDuration) {
            _elapsedTime -= CycleDuration;
            Cycle();
        }
    }

    public void Cycle() {
        LCDC lcdc = GetCurrentLCDC();

        if (!lcdc.IsDisplayEnabled) {
            _wasDisplayEnabled = false;
            _state = State.OAMSearch;
            _memory.InternalWriteByte(Memory.LY.Address, 0);
            _selectedOamEntriesCount = 0;
            UpdateLCDStatus(new());
            _stateCycleCounter = 0;
            _winLineCounter = 0;
            return;
        }

        if (!_wasDisplayEnabled) {
            for (uint i = 0; i < ScreenHeight; i++)
                for (uint j = 0; j < ScreenWidth; j++)
                    _screens.SetPixel(j, i, White);
        }

        switch (_state) {
            case State.OAMSearch:
                CycleOAMSearch();
                break;
            case State.PixelTransfer:
                CyclePixelTransfer();
                break;
            case State.HBlank:
                CycleHBlank();
                break;
            case State.VBlank:
                CycleVBlank();
                break;
        }
    }

    private bool TickDelay(int delay) {
        if (_stateCycleCounter != delay) {
            _stateCycleCounter++;
            return false;
        }

        _stateCycleCounter = 0;
        return true;
    }

    private void CycleOAMSearch() {
        if (!TickDelay(20))
            return;

        ReadOAMEntries();
        SelectOAMEntries();

        LCDC lcdc = GetCurrentLCDC();
        byte windowY = _memory.InternalReadByte(Memory.WY.Address);
        byte windowX = _memory.InternalReadByte(Memory.WX.Address);
        byte ly = _memory.InternalReadByte(Memory.LY.Address);
        if (lcdc.IsWindowEnabled && lcdc.IsBackgroundAndWindowEnable && windowY <= ly && windowX <= ScreenWidth)
            _winLineCounter++;

        _state = State.PixelTransfer;
        UpdateLCDStatus(new());
    }

    private void ReadOAMEntries() {
        ushort oamAddress = Memory.OAM.Address;
        for (int i = 0; i < _oamEntries.Length; i++, oamAddress += 4) {
            byte y = _memory.InternalReadByte(oamAddress);
            byte x = _memory.InternalReadByte((ushort)(oamAddress + 1));
            byte tileNumber = _memory.InternalReadByte((ushort)(oamAddress + 2));
            byte attributes = _memory.InternalReadByte((ushort)(oamAddress + 3));

            var entry = new OAMEntry(x, y, tileNumber, attributes);
            _oamEntries[i] = entry;
        }
    }

    private void SelectOAMEntries() {
        LCDC lcdc = GetCurrentLCDC();
        byte ly = _memory.InternalReadByte(Memory.LY.Address);

        _selectedOamEntriesCount = 0;
        foreach (var entry in _oamEntries) {
            short yPos = (short)(entry.Y - 16);
            if (yPos <= ly && ly < yPos + lcdc.ObjectSize) {
                _selectedOamEntries[_selectedOamEntriesCount] = entry;
                _selectedOamEntriesCount++;
                if (_selectedOamEntriesCount == 10)
                    break;
            }
        }
    }

    private void CyclePixelTransfer() {
        if (!TickDelay(43))
            return;

        byte lineY = _memory.InternalReadByte(Memory.LY.Address);
        LCDC lcdc = GetCurrentLCDC();

        for (byte x = 0; x < ScreenWidth; x++) {
            if (!_wasDisplayEnabled)
                break;

            byte colorDrewByBackgroundOrWindow = 0;
            if (lcdc.IsBackgroundAndWindowEnable) {
                colorDrewByBackgroundOrWindow = GetBgWinColor(lcdc, x, lineY);
                _screens.SetPixel(x, lineY, GetColorFromPalette(Memory.BGP.Address, colorDrewByBackgroundOrWindow));
            }

            if (lcdc.IsObjectEnabled) {
                OAMEntry selectedEntry = FindOAMEntry(x, colorDrewByBackgroundOrWindow);
                if (selectedEntry == null)
                    continue;

                short entryPositionX = (short)(selectedEntry.X - 8);
                short entryPositionY = (short)(selectedEntry.Y - 16);
                byte tileIndex = lcdc.ObjectSize == 16 ? (byte)(selectedEntry.Tile & 0xFE) : selectedEntry.Tile;
                byte tileLine = (byte)((lineY - entryPositionY) % lcdc.ObjectSize);
                if (selectedEntry.Flags.IsFlippedY)
                    tileLine = (byte)(lcdc.ObjectSize - tileLine - 1);

                if (tileLine >= 8) {
                    tileLine -= 8;
                    tileIndex |= 0x01;
                }

                byte tileColumn = (byte)((x - entryPositionX) % 8);
                ushort tileAddressInData = (ushort)(Memory.VRAM.Address + tileIndex * 16 + tileLine * 2);

                byte tileDataLow = _memory.InternalReadByte(tileAddressInData);
                byte tileDataHigh = _memory.InternalReadByte((ushort)(tileAddressInData + 1));

                if (!selectedEntry.Flags.IsFlippedX)
                    tileColumn = (byte)(7 - tileColumn);

                byte colorId = (byte)((((tileDataHigh >> tileColumn) & 0b1) << 1) | ((tileDataLow >> tileColumn) & 0b1));
                ushort paletteAddress = selectedEntry.Flags.Palette.Address;
                if (colorId == 0)
                    continue;
                _screens.SetPixel(x, lineY, GetColorFromPalette(paletteAddress, colorId));
            }
        }

        _state = State.HBlank;
        _memory.InternalWriteByte(Memory.LY.Address, (byte)(lineY + 1));
        UpdateLCDStatus(new() { LCDStatus.LCDStatusInterrupts.HBlankInt });
    }

    private OAMEntry FindOAMEntry(byte x, int colorDrewByBackgroundOrWindow) {
        var minX = int.MaxValue;
        OAMEntry selectedEntry = null;

        for (int i = 0; i < _selectedOamEntriesCount; i++) {
            var entry = _selectedOamEntries[i];

            if (entry.Flags.IsBehindBackground && colorDrewByBackgroundOrWindow != 0)
                continue;

            short entryPositionX = (short)(entry.X - 8);
            if (entryPositionX < minX && x >= entryPositionX && x < entryPositionX + 8) {
                selectedEntry = entry;
                minX = entryPositionX;
            }
        }
        return selectedEntry;
    }

    private byte GetBgWinColor(LCDC lcdc, byte x, byte lineY) {
        byte scrollX = _memory.InternalReadByte(Memory.SCX.Address);
        byte scrollY = _memory.InternalReadByte(Memory.SCY.Address);
        byte windowX = (byte)(_memory.InternalReadByte(Memory.WX.Address) - 7);
        byte windowY = _memory.InternalReadByte(Memory.WY.Address);

        bool useWindowThisLine = windowY <= lineY && lcdc.IsWindowEnabled && lcdc.IsBackgroundAndWindowEnable;
        bool useWindowThisPixel = useWindowThisLine && windowX <= x;

        ushort tileMapAddress = useWindowThisPixel ? lcdc.WindowTileMap : lcdc.BackgroundTileMap;

        byte yPosition = useWindowThisPixel ? (byte)(_winLineCounter - 1) : (byte)(lineY + scrollY);
        byte xPosition = useWindowThisPixel ? (byte)(x - windowX) : (byte)(x + scrollX);
        byte tileRow = (byte)(yPosition / 8);
        byte tileColumn = (byte)(xPosition / 8);

        ushort tileAddressInMap = (ushort)(tileMapAddress + tileRow * 32 + tileColumn);
        short tileOffsetInData = _memory.InternalReadByte(tileAddressInMap);

        ushort tileAddressInData = lcdc.BackgroundTileSet;
        if (lcdc.BackgroundTileSet == 0x8000)
            tileAddressInData += (ushort)(tileOffsetInData * 16);
        else
            tileAddressInData += (ushort)((sbyte)tileOffsetInData * 16);

        byte tileLine = (byte)(yPosition % 8);
        byte tileColumnInLine = (byte)(xPosition % 8);

        byte tileDataLow = _memory.InternalReadByte((ushort)(tileAddressInData + tileLine * 2));
        byte tileDataHigh = _memory.InternalReadByte((ushort)(tileAddressInData + tileLine * 2 + 1));

        byte colorIndex = (byte)(((tileDataHigh >> (7 - tileColumnInLine)) & 0b1) << 1);
        colorIndex |= (byte)((tileDataLow >> (7 - tileColumnInLine)) & 0b1);

        return colorIndex;
    }

    private void CycleHBlank() {
        if (!TickDelay(51))
            return;

        byte lineY = _memory.InternalReadByte(Memory.LY.Address);
        _state = lineY == ScreenHeight ? State.VBlank : State.OAMSearch;
        if (_state == State.VBlank) {
            _memory.RequestInterrupt(Interrupt.VBlank);
            _screens.Swap();
            _wasDisplayEnabled = true;
        }

        var stateInterruptToCheck = lineY == ScreenHeight ? LCDStatus.LCDStatusInterrupts.VBlankInt : LCDStatus.LCDStatusInterrupts.OAMSearchInt;
        UpdateLCDStatus(new() {
            stateInterruptToCheck,
            LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt
        });
    }

    private void CycleVBlank() {
        if (!TickDelay(114 * 10)) {
            if (_stateCycleCounter % 114 == 0) {
                byte lineY = _memory.InternalReadByte(Memory.LY.Address);
                _memory.InternalWriteByte(Memory.LY.Address, (byte)(lineY + 1));
                UpdateLCDStatus(new() { LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt });
            }
            return;
        }

        _memory.InternalWriteByte(Memory.LY.Address, 0);
        _winLineCounter = 0;
        _state = State.OAMSearch;
        UpdateLCDStatus(new() {
            LCDStatus.LCDStatusInterrupts.OAMSearchInt,
            LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt,
            LCDStatus.LCDStatusInterrupts.VBlankInt
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearOAMEntries() {
        for (int i = 0; i < 40; i++)
            _oamEntries[i] = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint GetColorFromPalette(ushort paletteAddress, byte colorIndex, bool transparent = false) {
        if (transparent && colorIndex == 0)
            return 0x00000000;
        byte palette = _memory.InternalReadByte(paletteAddress);
        byte color = (byte)((palette >> (colorIndex * 2)) & 0b11);
        return Colors[color];
    }

    private void UpdateLCDStatus(HashSet<LCDStatus.LCDStatusInterrupts> interruptsToCheck) {
        var lcdStatus = GetCurrentLCDStatus();
        byte lineY = _memory.InternalReadByte(Memory.LY.Address);
        byte lyc = _memory.InternalReadByte(Memory.LYC.Address);

        lcdStatus.SetMode(LCDStatus.GetModeFromPPUState(_state));
        lcdStatus.SetInterrupt(LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt, lineY == lyc);
        _memory.InternalWriteByte(Memory.STAT.Address, lcdStatus.Value);

        bool coincidenceInterrupt = lcdStatus.HasLycLyCoincidenceInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt);
        bool oamSearchInterrupt = lcdStatus.HasOAMSearchInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.OAMSearchInt);
        bool vBlankInterrupt = lcdStatus.HasVBlankInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.VBlankInt);
        bool hBlankInterrupt = lcdStatus.HasHBlankInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.HBlankInt);

        if (coincidenceInterrupt || oamSearchInterrupt || vBlankInterrupt || hBlankInterrupt) {
            _memory.RequestInterrupt(Interrupt.LCDStat);
        }
    }

    public const uint ScreenWidth = 160;
    public const uint ScreenHeight = 144;
    public const uint White = 0xFFFFFF;
    public const uint LightGray = 0xAAAAAA;
    public const uint DarkGray = 0x555555;
    public const uint Black = 0x000000;
    public static readonly uint[] Colors = new uint[] { White, LightGray, DarkGray, Black };

    public const float ClockSpeed = 2_457_600f;
    public const float CycleDuration = 1f / ClockSpeed;
}
