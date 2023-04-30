namespace GBMU.Core;

public partial class CPU
{
	public byte A, B, C, D, E, H, L;
	private byte _f;
	private ushort _sp, _pc;

	public CPU()
	{
		InitializeRegisters();
	}

	private void InitializeRegisters()
	{
		AF = AFBaseValue;
		BC = BCBaseValue;
		DE = DEBaseValue;
		HL = HLBaseValue;
		SP = SPBaseValue;
		PC = PCBaseValue;
	}

	public ushort AF
	{
		get => ByteUtils.MakeWord(A, _f);
		set => Set16BitRegister(ref A, ref _f, value);
	}

	public ushort BC
	{
		get => ByteUtils.MakeWord(B, C);
		set => Set16BitRegister(ref B, ref C, value);
	}

	public ushort DE
	{
		get => ByteUtils.MakeWord(D, E);
		set => Set16BitRegister(ref D, ref E, value);
	}

	public ushort HL
	{
		get => ByteUtils.MakeWord(H, L);
		set => Set16BitRegister(ref H, ref L, value);
	}

	public ushort SP
	{
		get => _sp;
		set => _sp = value;
	}

	public ushort PC
	{
		get => _pc;
		set => _pc = value;
	}

	private void Set16BitRegister(ref byte high, ref byte low, ushort value)
	{
		high = ByteUtils.HighByte(value);
		low = ByteUtils.LowByte(value);
	}

	public void ResetFlags()
	{
		_f = 0;
	}

	public void SetFlag(CPUFlag flag, bool value)
	{
		if (value)
			_f |= (byte)flag;
		else
			_f &= (byte)~flag;
	}

	public bool GetFlag(CPUFlag flag) => (_f & (byte)flag) != 0;

	public static ushort AFBaseValue = 0x01B0;
	public static ushort BCBaseValue = 0x0013;
	public static ushort DEBaseValue = 0x00D8;
	public static ushort HLBaseValue = 0x014D;
	public static ushort SPBaseValue = 0xFFFE;
	public static ushort PCBaseValue = 0x0100;
}