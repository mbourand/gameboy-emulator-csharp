using System.IO;

namespace GBMU.Core;

public class Cartridge
{
	private byte[] _rom;

	public Cartridge(Stream stream)
	{
		_rom = new byte[stream.Length];
		stream.Read(_rom, 0, _rom.Length);
	}

	public int RomSize => _rom.Length;

	public byte ReadByte(uint addr) => MemoryUtils.ReadByte(_rom, addr);
	public ushort ReadWord(uint addr) => MemoryUtils.ReadWord(_rom, addr);

	public void WriteByte(uint addr, byte value) => MemoryUtils.WriteByte(_rom, addr, value);

	public static MemoryKeyPoint EntryPoint = new MemoryKeyPoint(0x100, 4);
	public static MemoryKeyPoint NintendoLogo = new MemoryKeyPoint(0x104, 48);
	public static MemoryKeyPoint Title = new MemoryKeyPoint(0x134, 16);
	public static MemoryKeyPoint ManufacturerCode = new MemoryKeyPoint(0x13F, 4);
	public static MemoryKeyPoint CGBFlag = new MemoryKeyPoint(0x143, 1);
	public static MemoryKeyPoint NewLicenseeCode = new MemoryKeyPoint(0x144, 2);
	public static MemoryKeyPoint SGBFlag = new MemoryKeyPoint(0x146, 1);
	public static MemoryKeyPoint CartridgeType = new MemoryKeyPoint(0x147, 1);
	public static MemoryKeyPoint ROMSize = new MemoryKeyPoint(0x148, 1);
	public static MemoryKeyPoint RAMSize = new MemoryKeyPoint(0x149, 1);
	public static MemoryKeyPoint DestinationCode = new MemoryKeyPoint(0x14A, 1);
	public static MemoryKeyPoint OldLicenseeCode = new MemoryKeyPoint(0x14B, 1);
	public static MemoryKeyPoint MaskROMVersionNumber = new MemoryKeyPoint(0x14C, 1);
	public static MemoryKeyPoint HeaderChecksum = new MemoryKeyPoint(0x14D, 1);
	public static MemoryKeyPoint GlobalChecksum = new MemoryKeyPoint(0x14E, 2);
}