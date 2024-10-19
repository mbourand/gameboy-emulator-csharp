using System.IO;

namespace GBMU.Core;

public class Cartridge {
    public readonly byte[] Rom;

    public Cartridge(Stream stream) {
        Rom = new byte[stream.Length];
        stream.Read(Rom, 0, Rom.Length);
    }

    public int RomSize => Rom.Length;
    public int RomBankCount => 1 << (MemoryUtils.ReadByte(Rom, ROMSize.Address) + 1);

    public IMemoryHook GetMemoryBankController() {
        byte cartridgeType = MemoryUtils.ReadByte(Rom, CartridgeType.Address);
        switch (cartridgeType) {
            case 0x00:
            case 0x08:
            case 0x09:
                return null; //new NoMBC(this);
            case 0x01:
            case 0x02:
            case 0x03:
                return new MBC1(this);
            default:
                return null;
        }
    }

    public byte ReadByte(ushort addr) => MemoryUtils.ReadByte(Rom, addr);
    public ushort ReadWord(ushort addr) => MemoryUtils.ReadWord(Rom, addr);

    public void WriteByte(ushort addr, byte value) => MemoryUtils.WriteByte(Rom, addr, value);
    public void WriteWord(ushort addr, ushort value) => MemoryUtils.WriteWord(Rom, addr, value);

    public static readonly MemoryKeyPoint EntryPoint = new(0x100, 4);
    public static readonly MemoryKeyPoint NintendoLogo = new(0x104, 48);
    public static readonly MemoryKeyPoint Title = new(0x134, 16);
    public static readonly MemoryKeyPoint ManufacturerCode = new(0x13F, 4);
    public static readonly MemoryKeyPoint CGBFlag = new(0x143, 1);
    public static readonly MemoryKeyPoint NewLicenseeCode = new(0x144, 2);
    public static readonly MemoryKeyPoint SGBFlag = new(0x146, 1);
    public static readonly MemoryKeyPoint CartridgeType = new(0x147, 1);
    public static readonly MemoryKeyPoint ROMSize = new(0x148, 1);
    public static readonly MemoryKeyPoint RAMSize = new(0x149, 1);
    public static readonly MemoryKeyPoint DestinationCode = new(0x14A, 1);
    public static readonly MemoryKeyPoint OldLicenseeCode = new(0x14B, 1);
    public static readonly MemoryKeyPoint MaskROMVersionNumber = new(0x14C, 1);
    public static readonly MemoryKeyPoint HeaderChecksum = new(0x14D, 1);
    public static readonly MemoryKeyPoint GlobalChecksum = new(0x14E, 2);
}
