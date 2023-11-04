using System.Collections.Generic;
using Microsoft.VisualBasic;

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
	private readonly uint[][] _screen;
	private readonly OAMEntry[] _oamEntries;
	private readonly List<OAMEntry> _selectedOamEntries = new();

	private int _stateCycleCounter;
	private double _elapsedTime;

	private int _winLineCounter;

	public PPU(Memory memory) {
		_memory = memory;

		_state = State.VBlank;
		_stateCycleCounter = 0;
		_winLineCounter = 0;

		_screen = new uint[ScreenHeight][];
		for (int i = 0; i < ScreenHeight; i++)
			_screen[i] = new uint[ScreenWidth];

		for (int i = 0; i < ScreenHeight; i++)
			for (int j = 0; j < 144; j++)
				_screen[i][j] = White;

		_oamEntries = new OAMEntry[40];
	}

	public LCDC GetCurrentLCDC() => new(_memory.ReadByte(Memory.LCDC.Address));
	public LCDStatus GetCurrentLCDStatus() => new(_memory.ReadByte(Memory.STAT.Address));

	public uint[][] GetScreen() => _screen;

	public void Update(double deltaTime) {
		_elapsedTime += deltaTime;
		while (_elapsedTime >= CycleDuration) {
			_elapsedTime -= CycleDuration;
			Cycle();
		}
	}

	public void Cycle() {
		LCDC lcdc = GetCurrentLCDC();
		if (!lcdc.IsDisplayEnabled) {
			_state = State.OAMSearch;
			_memory.WriteByte(Memory.LY.Address, 0);
			_selectedOamEntries.Clear();
			UpdateLCDStatus(new());
			_stateCycleCounter = 0;
			_winLineCounter = 0;
			return;
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

		ClearOAMEntries();
		ReadOAMEntries();
		SelectOAMEntries();

		LCDC lcdc = GetCurrentLCDC();
		byte windowY = _memory.ReadByte(Memory.WY.Address);
		byte windowX = _memory.ReadByte(Memory.WX.Address);
		byte ly = _memory.ReadByte(Memory.LY.Address, false);
		if (lcdc.IsWindowEnabled && lcdc.IsBackgroundAndWindowEnable && windowY <= ly && windowX <= ScreenWidth)
			_winLineCounter++;

		_state = State.PixelTransfer;
		UpdateLCDStatus(new());
	}

	private void ReadOAMEntries() {
		ushort oamAddress = Memory.OAM.Address;
		for (int i = 0; i < _oamEntries.Length; i++, oamAddress += 4) {
			byte y = _memory.ReadByte(oamAddress);
			byte x = _memory.ReadByte((ushort)(oamAddress + 1));
			byte tileNumber = _memory.ReadByte((ushort)(oamAddress + 2));
			byte attributes = _memory.ReadByte((ushort)(oamAddress + 3));

			var entry = new OAMEntry(y, x, tileNumber, attributes);
			_oamEntries[i] = entry;
		}
	}

	private void SelectOAMEntries() {
		LCDC lcdc = GetCurrentLCDC();
		byte ly = _memory.ReadByte(Memory.LY.Address, false);

		_selectedOamEntries.Clear();
		foreach (var entry in _oamEntries) {
			short yPos = (short)(entry.Y - 16);
			if (yPos <= ly && ly < yPos + lcdc.ObjectSize) {
				_selectedOamEntries.Add(entry);
				if (_selectedOamEntries.Count == 10)
					break;
			}
		}
	}

	private void CyclePixelTransfer() {
		if (!TickDelay(43))
			return;

		byte lineY = _memory.ReadByte(Memory.LY.Address, false);

		byte scrollX = _memory.ReadByte(Memory.SCX.Address);
		byte scrollY = _memory.ReadByte(Memory.SCY.Address);
		byte windowX = _memory.ReadByte(Memory.WX.Address);
		byte windowY = _memory.ReadByte(Memory.WY.Address);
		LCDC lcdc = GetCurrentLCDC();

		bool useWindowThisLine = lcdc.IsWindowEnabled && lcdc.IsBackgroundAndWindowEnable && windowY <= lineY;

		for (byte x = 0; x < ScreenWidth; x++) {
			bool useWindowThisPixel = useWindowThisLine && windowX <= x;
			ushort tileMapAddress = useWindowThisPixel ? lcdc.WindowTileMap : lcdc.BackgroundTileMap;

			byte yPosition = useWindowThisPixel ? (byte)(_winLineCounter - 1) : (byte)(lineY + scrollY);
			byte xPosition = useWindowThisPixel ? (byte)(x - windowX) : (byte)(x + scrollX);
			byte tileRow = (byte)(yPosition / 8);
			byte tileColumn = (byte)(xPosition / 8);

			ushort tileAddressInMap = (ushort)(tileMapAddress + tileRow * 32 + tileColumn);
			short tileOffsetInData = _memory.ReadByte(tileAddressInMap);

			ushort tileAddressInData = lcdc.BackgroundTileSet;
			if (lcdc.BackgroundTileSet == 0x8000)
				tileAddressInData += (ushort)(tileOffsetInData * 16);
			else
				tileAddressInData += (ushort)((sbyte)tileOffsetInData * 16);

			byte tileLine = (byte)(yPosition % 8);
			byte tileColumnInLine = (byte)(xPosition % 8);

			byte tileDataLow = _memory.ReadByte((ushort)(tileAddressInData + tileLine * 2));
			byte tileDataHigh = _memory.ReadByte((ushort)(tileAddressInData + tileLine * 2 + 1));

			byte colorIndex = (byte)(((tileDataHigh >> (7 - tileColumnInLine)) & 0b1) << 1);
			colorIndex |= (byte)((tileDataLow >> (7 - tileColumnInLine)) & 0b1);

			_screen[lineY][x] = GetColorFromPalette(colorIndex);
		}

		_state = State.HBlank;
		_memory.WriteByte(Memory.LY.Address, (byte)(lineY + 1));
		UpdateLCDStatus(new() { LCDStatus.LCDStatusInterrupts.HBlankInt });
	}

	private void CycleHBlank() {
		if (!TickDelay(51))
			return;

		byte lineY = _memory.ReadByte(Memory.LY.Address, false);
		_state = lineY == 144 ? State.VBlank : State.OAMSearch;
		if (_state == State.VBlank) {
			var newIf = (byte)(_memory.ReadByte(Memory.InterruptFlagRegister.Address) | (byte)Interrupt.VBlank);
			_memory.WriteByte(Memory.InterruptFlagRegister.Address, newIf);
		}

		var stateInterruptToCheck = lineY == 144 ? LCDStatus.LCDStatusInterrupts.VBlankInt : LCDStatus.LCDStatusInterrupts.OAMSearchInt;
		UpdateLCDStatus(new() {
			stateInterruptToCheck,
			LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt
		});
	}

	private void CycleVBlank() {
		if (!TickDelay(114 * 10)) {
			if (_stateCycleCounter % 114 == 0) {
				byte lineY = _memory.ReadByte(Memory.LY.Address, false);
				_memory.WriteByte(Memory.LY.Address, (byte)(lineY + 1));
				UpdateLCDStatus(new() { LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt });
			}
			return;
		}

		_memory.WriteByte(Memory.LY.Address, 0);
		_winLineCounter = 0;
		_state = State.OAMSearch;
		UpdateLCDStatus(new() {
			LCDStatus.LCDStatusInterrupts.OAMSearchInt,
			LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt,
			LCDStatus.LCDStatusInterrupts.VBlankInt
		});
	}

	private void ClearOAMEntries() {
		for (int i = 0; i < 40; i++)
			_oamEntries[i] = null;
	}

	private uint GetColorFromPalette(byte colorIndex, bool transparent = false) {
		if (transparent && colorIndex == 0)
			return 0x00000000;
		byte palette = _memory.ReadByte(Memory.BGP.Address);
		byte color = (byte)((palette >> (colorIndex * 2)) & 0b11);
		return Colors[color];
	}

	private void UpdateLCDStatus(HashSet<LCDStatus.LCDStatusInterrupts> interruptsToCheck) {
		var lcdStatus = GetCurrentLCDStatus();
		byte lineY = _memory.ReadByte(Memory.LY.Address);
		byte lyc = _memory.ReadByte(Memory.LYC.Address);

		lcdStatus.SetMode(LCDStatus.GetModeFromPPUState(_state));
		lcdStatus.SetInterrupt(LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt, lineY == lyc);
		_memory.WriteByte(Memory.STAT.Address, lcdStatus.Value);

		bool coincidenceInterrupt = lcdStatus.HasLycLyCoincidenceInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.LycLyCoincidenceInt);
		bool oamSearchInterrupt = lcdStatus.HasOAMSearchInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.OAMSearchInt);
		bool vBlankInterrupt = lcdStatus.HasVBlankInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.VBlankInt);
		bool hBlankInterrupt = lcdStatus.HasHBlankInt && interruptsToCheck.Contains(LCDStatus.LCDStatusInterrupts.HBlankInt);

		if (coincidenceInterrupt || oamSearchInterrupt || vBlankInterrupt || hBlankInterrupt) {
			byte newIf = (byte)(_memory.ReadByte(Memory.InterruptFlagRegister.Address) | (byte)Interrupt.LCDStat);
			_memory.WriteByte(Memory.InterruptFlagRegister.Address, newIf);
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