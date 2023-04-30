namespace GBMU.Core;

public class Memory
{
	private byte[] _memory;
	private Cartridge _cartridge;

	public Memory(Cartridge cartridge)
	{
		_memory = new byte[0x10000];
		_cartridge = cartridge;
	}

	public byte ReadByte(ushort addr)
	{
		if (IsInCartridgeArea(addr))
			return _cartridge != null ? _cartridge.ReadByte(addr) : (byte)0xFF;
		if (Echo.IsInRange(addr))
			return MemoryUtils.RedirectedReadByte(_memory, addr, Unusable.addr, ROMBank0.addr);
		return MemoryUtils.ReadByte(_memory, addr);
	}


	public void WriteByte(ushort addr, byte value)
	{
		MemoryUtils.WriteByte(_memory, addr, value);
		//if (IsInCartridgeArea(addr))
		// Write handled by MBC
		if (Echo.IsInRange(addr))
			MemoryUtils.RedirectedWriteByte(_memory, addr, Echo.addr, ROMBank0.addr, value);
	}

	public ushort ReadWord(ushort addr)
	{
		ushort word = ByteUtils.MakeWord(ReadByte(addr), ReadByte((ushort)(addr + 1)));
		return ByteUtils.FlipEndian(word);
	}

	public void WriteWord(ushort addr, ushort value)
	{
		var flipped = ByteUtils.FlipEndian(value);
		WriteByte(addr, ByteUtils.HighByte(flipped));
		WriteByte((ushort)(addr + 1), ByteUtils.LowByte(flipped));
	}

	private bool IsInCartridgeArea(ushort addr) => ROMBank0.IsInRange(addr) || ExternalRAM.IsInRange(addr);

	public static MemoryKeyPoint ROMBank0 = new MemoryKeyPoint(0x0000, 0x4000);
	public static MemoryKeyPoint SwitchableROMBank = new MemoryKeyPoint(0x4000, 0x4000);
	public static MemoryKeyPoint VRAM = new MemoryKeyPoint(0x8000, 0x2000);
	public static MemoryKeyPoint ExternalRAM = new MemoryKeyPoint(0xA000, 0x2000);
	public static MemoryKeyPoint WorkRAMBank0 = new MemoryKeyPoint(0xC000, 0x1000);
	public static MemoryKeyPoint SwitchableWorkRAMBank = new MemoryKeyPoint(0xD000, 0x1000);
	public static MemoryKeyPoint Echo = new MemoryKeyPoint(0xE000, 0x1E00);
	public static MemoryKeyPoint OAM = new MemoryKeyPoint(0xFE00, 0xA0);
	public static MemoryKeyPoint Unusable = new MemoryKeyPoint(0xFEA0, 0x60);
	public static MemoryKeyPoint IORegisters = new MemoryKeyPoint(0xFF00, 0x80);
	public static MemoryKeyPoint HRAM = new MemoryKeyPoint(0xFF80, 0x7F);
	public static MemoryKeyPoint InterruptEnableRegister = new MemoryKeyPoint(0xFFFF, 1);
}