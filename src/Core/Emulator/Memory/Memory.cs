using System.Collections.Generic;

namespace GBMU.Core;

public class Memory {
    private readonly byte[] _memory;
    private readonly List<IMemoryHook> _hooks = new();

    public Memory(Cartridge cartridge) {
        _memory = new byte[0x10000];
        for (int i = 0; i < _memory.Length; i++) {
            _memory[i] = 0xFF;
        }
        for (ushort i = CartridgeSpan.Address; i < CartridgeSpan.EndAddress; i++) {
            _memory[i] = cartridge.ReadByte((ushort)(i - CartridgeSpan.Address));
        }

        _memory[P1.Address] = 0xCF;
        _memory[SB.Address] = 0x00;
        _memory[SC.Address] = 0x7E;
        _memory[DIV.Address] = 0x18;
        _memory[TIMA.Address] = 0x00;
        _memory[TMA.Address] = 0x00;
        _memory[TAC.Address] = 0xF8;
        _memory[InterruptFlagRegister.Address] = 0xE1;
        _memory[NR10.Address] = 0x80;
        _memory[NR11.Address] = 0xBF;
        _memory[NR12.Address] = 0xF3;
        _memory[NR13.Address] = 0xFF;
        _memory[NR14.Address] = 0xBF;
        _memory[NR21.Address] = 0x3F;
        _memory[NR22.Address] = 0x00;
        _memory[NR23.Address] = 0xFF;
        _memory[NR24.Address] = 0xBF;
        _memory[NR30.Address] = 0x7F;
        _memory[NR31.Address] = 0xFF;
        _memory[NR32.Address] = 0x9F;
        _memory[NR33.Address] = 0xFF;
        _memory[NR34.Address] = 0xBF;
        _memory[NR41.Address] = 0xFF;
        _memory[NR42.Address] = 0x00;
        _memory[NR43.Address] = 0x00;
        _memory[NR44.Address] = 0xBF;
        _memory[NR50.Address] = 0x77;
        _memory[NR51.Address] = 0xF3;
        _memory[NR52.Address] = 0xF1;
        _memory[LCDC.Address] = 0x91;
        _memory[STAT.Address] = 0x81;
        _memory[SCY.Address] = 0x00;
        _memory[SCX.Address] = 0x00;
        _memory[LY.Address] = 0x91;
        _memory[LYC.Address] = 0x00;
        _memory[DMA.Address] = 0xFF;
        _memory[BGP.Address] = 0xFC;
        _memory[OBP0.Address] = 0x00;
        _memory[OBP1.Address] = 0x00;
        _memory[WY.Address] = 0x00;
        _memory[WX.Address] = 0x00;
        _memory[KEY1.Address] = 0xFF;
        _memory[VBK.Address] = 0xFF;
        _memory[HDMA1.Address] = 0xFF;
        _memory[HDMA2.Address] = 0xFF;
        _memory[HDMA3.Address] = 0xFF;
        _memory[HDMA4.Address] = 0xFF;
        _memory[HDMA5.Address] = 0xFF;
        _memory[RP.Address] = 0xFF;
        _memory[BCPS.Address] = 0xFF;
        _memory[BCPD.Address] = 0xFF;
        _memory[OCPS.Address] = 0xFF;
        _memory[OCPD.Address] = 0xFF;
        _memory[SVBK.Address] = 0xFF;
        _memory[InterruptEnableRegister.Address] = 0x00;
    }

    public void RegisterHook(IMemoryHook hook) {
        _hooks.Add(hook);
    }

    public void UnregisterHook(IMemoryHook hook) {
        _hooks.Remove(hook);
    }

    public void InternalWriteByte(ushort addr, byte value) {
        MemoryUtils.WriteByte(_memory, addr, value);
    }

    public void InternalWriteWord(ushort addr, ushort value) {
        MemoryUtils.WriteWord(_memory, addr, value);
    }
    public byte InternalReadByte(ushort addr) => MemoryUtils.ReadByte(_memory, addr);
    public ushort InternalReadWord(ushort addr) => MemoryUtils.ReadWord(_memory, addr);

    public byte ReadByte(ushort addr) {
        foreach (var hook in _hooks) {
            var value = hook.OnReadByte(_memory, addr);
            if (value.HasValue)
                return value.Value;
        }
        return MemoryUtils.ReadByte(_memory, addr);
    }

    public void WriteByte(ushort addr, byte value) {
        foreach (var hook in _hooks)
            if (hook.OnWriteByte(_memory, addr, value))
                return;
        MemoryUtils.WriteByte(_memory, addr, value);
    }

    public ushort ReadWord(ushort addr) {
        foreach (var hook in _hooks) {
            var value = hook.OnReadWord(_memory, addr);
            if (value.HasValue)
                return value.Value;
        }
        ushort word = ByteUtils.MakeWord(ReadByte(addr), ReadByte((ushort)(addr + 1)));
        return ByteUtils.FlipEndian(word);
    }

    public void WriteWord(ushort addr, ushort value) {
        foreach (var hook in _hooks)
            if (hook.OnWriteWord(_memory, addr, value))
                return;
        var flipped = ByteUtils.FlipEndian(value);
        WriteByte(addr, ByteUtils.HighByte(flipped));
        WriteByte((ushort)(addr + 1), ByteUtils.LowByte(flipped));
    }

    public string Dump() {
        string dump = "     | 00 01 02 03 04 05 06 07 | 08 09 0A 0B 0C 0D 0E 0F\n";
        dump += "-----|-------------------------|------------------------";

        for (int i = 0; i < _memory.Length; i++) {
            if (i % 16 == 0)
                dump += $"\n{i:X4} | ";
            if (i % 8 == 0 && i % 16 != 0)
                dump += "| ";
            dump += $"{ReadByte((ushort)i):X2} ";
        }
        return dump;
    }

    public void RequestInterrupt(Interrupt interrupt) {
        byte interruptFlag = ReadByte(InterruptFlagRegister.Address);
        interruptFlag |= (byte)interrupt;
        WriteByte(InterruptFlagRegister.Address, interruptFlag);
    }

    public static readonly MemoryKeyPoint InterruptVectorVBlank = new(0x0040, 1);
    public static readonly MemoryKeyPoint InterruptVectorLCDStat = new(0x0048, 1);
    public static readonly MemoryKeyPoint InterruptVectorTimer = new(0x0050, 1);
    public static readonly MemoryKeyPoint InterruptVectorSerial = new(0x0058, 1);
    public static readonly MemoryKeyPoint InterruptVectorJoypad = new(0x0060, 1);

    public static readonly MemoryKeyPoint ROMBank0 = new(0x0000, 0x4000);
    public static readonly MemoryKeyPoint SwitchableROMBank = new(0x4000, 0x4000);
    public static readonly MemoryKeyPoint VRAM = new(0x8000, 0x2000);
    public static readonly MemoryKeyPoint ExternalRAM = new(0xA000, 0x2000);
    public static readonly MemoryKeyPoint WorkRAMBank0 = new(0xC000, 0x1000);
    public static readonly MemoryKeyPoint SwitchableWorkRAMBank = new(0xD000, 0x1000);
    public static readonly MemoryKeyPoint Echo = new(0xE000, 0x1E00);
    public static readonly MemoryKeyPoint OAM = new(0xFE00, 0xA0);
    public static readonly MemoryKeyPoint Unusable = new(0xFEA0, 0x60);
    public static readonly MemoryKeyPoint IORegisters = new(0xFF00, 0x80);
    public static readonly MemoryKeyPoint HRAM = new(0xFF80, 0x7F);
    public static readonly MemoryKeyPoint InterruptEnableRegister = new(0xFFFF, 1);

    public static readonly MemoryKeyPoint P1 = new(0xFF00, 1);
    public static readonly MemoryKeyPoint SB = new(0xFF01, 1);
    public static readonly MemoryKeyPoint SC = new(0xFF02, 1);
    public static readonly MemoryKeyPoint DIV = new(0xFF04, 1);
    public static readonly MemoryKeyPoint TIMA = new(0xFF05, 1);
    public static readonly MemoryKeyPoint TMA = new(0xFF06, 1);
    public static readonly MemoryKeyPoint TAC = new(0xFF07, 1);
    public static readonly MemoryKeyPoint InterruptFlagRegister = new(0xFF0F, 1);
    public static readonly MemoryKeyPoint NR10 = new(0xFF10, 1);
    public static readonly MemoryKeyPoint NR11 = new(0xFF11, 1);
    public static readonly MemoryKeyPoint NR12 = new(0xFF12, 1);
    public static readonly MemoryKeyPoint NR13 = new(0xFF13, 1);
    public static readonly MemoryKeyPoint NR14 = new(0xFF14, 1);
    public static readonly MemoryKeyPoint NR21 = new(0xFF16, 1);
    public static readonly MemoryKeyPoint NR22 = new(0xFF17, 1);
    public static readonly MemoryKeyPoint NR23 = new(0xFF18, 1);
    public static readonly MemoryKeyPoint NR24 = new(0xFF19, 1);
    public static readonly MemoryKeyPoint NR30 = new(0xFF1A, 1);
    public static readonly MemoryKeyPoint NR31 = new(0xFF1B, 1);
    public static readonly MemoryKeyPoint NR32 = new(0xFF1C, 1);
    public static readonly MemoryKeyPoint NR33 = new(0xFF1D, 1);
    public static readonly MemoryKeyPoint NR34 = new(0xFF1E, 1);
    public static readonly MemoryKeyPoint NR41 = new(0xFF20, 1);
    public static readonly MemoryKeyPoint NR42 = new(0xFF21, 1);
    public static readonly MemoryKeyPoint NR43 = new(0xFF22, 1);
    public static readonly MemoryKeyPoint NR44 = new(0xFF23, 1);
    public static readonly MemoryKeyPoint NR50 = new(0xFF24, 1);
    public static readonly MemoryKeyPoint NR51 = new(0xFF25, 1);
    public static readonly MemoryKeyPoint NR52 = new(0xFF26, 1);
    public static readonly MemoryKeyPoint LCDC = new(0xFF40, 1);
    public static readonly MemoryKeyPoint STAT = new(0xFF41, 1);
    public static readonly MemoryKeyPoint SCY = new(0xFF42, 1);
    public static readonly MemoryKeyPoint SCX = new(0xFF43, 1);
    public static readonly MemoryKeyPoint LY = new(0xFF44, 1);
    public static readonly MemoryKeyPoint LYC = new(0xFF45, 1);
    public static readonly MemoryKeyPoint DMA = new(0xFF46, 1);
    public static readonly MemoryKeyPoint BGP = new(0xFF47, 1);
    public static readonly MemoryKeyPoint OBP0 = new(0xFF48, 1);
    public static readonly MemoryKeyPoint OBP1 = new(0xFF49, 1);
    public static readonly MemoryKeyPoint WY = new(0xFF4A, 1);
    public static readonly MemoryKeyPoint WX = new(0xFF4B, 1);
    public static readonly MemoryKeyPoint KEY1 = new(0xFF4D, 1);
    public static readonly MemoryKeyPoint VBK = new(0xFF4F, 1);
    public static readonly MemoryKeyPoint HDMA1 = new(0xFF51, 1);
    public static readonly MemoryKeyPoint HDMA2 = new(0xFF52, 1);
    public static readonly MemoryKeyPoint HDMA3 = new(0xFF53, 1);
    public static readonly MemoryKeyPoint HDMA4 = new(0xFF54, 1);
    public static readonly MemoryKeyPoint HDMA5 = new(0xFF55, 1);
    public static readonly MemoryKeyPoint RP = new(0xFF56, 1);
    public static readonly MemoryKeyPoint BCPS = new(0xFF68, 1);
    public static readonly MemoryKeyPoint BCPD = new(0xFF69, 1);
    public static readonly MemoryKeyPoint OCPS = new(0xFF6A, 1);
    public static readonly MemoryKeyPoint OCPD = new(0xFF6B, 1);
    public static readonly MemoryKeyPoint SVBK = new(0xFF70, 1);
    public static readonly MemoryKeyPoint RAMEnable = new(0x0000, 0x2000);
    public static readonly MemoryKeyPoint ROMBankNumber = new(0x2000, 0x2000);
    public static readonly MemoryKeyPoint RAMBankNumber = new(0x4000, 0x2000);
    public static readonly MemoryKeyPoint BankingModeSelect = new(0x6000, 0x2000);
    public static readonly MemoryKeyPoint CartridgeSpan = new(0x0000, 0x8000);
}
