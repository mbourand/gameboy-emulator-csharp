namespace GBMU.Core;

public enum CPUFlag
{
	ZERO = 0b1000_0000,
	N_SUBTRACT = 0b0100_0000,
	HALF_CARRY = 0b0010_0000,
	CARRY = 0b0001_0000
}

public static class CPUFlagUtils
{
	public static string GetChar(this CPUFlag flag) => flag switch
	{
		CPUFlag.ZERO => "Z",
		CPUFlag.N_SUBTRACT => "N",
		CPUFlag.HALF_CARRY => "H",
		CPUFlag.CARRY => "C",
		_ => throw new System.Exception("Invalid flag")
	};
}