namespace GBMU.Core;

public class OperatorJRFlagI8 : CPUOperator
{
	public OperatorJRFlagI8() : base("ADD", 2) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var flag = GetFlag(opcode);
		var isFlagSet = cpu.GetFlag(flag);

		if (isFlagSet == ExpectedValue(opcode))
		{
			var offset = (sbyte)memory.ReadByte((ushort)(cpu.PC + 1));
			cpu.PC = (ushort)(cpu.PC + offset);
		}

		base.Execute(cpu, memory, opcode);
	}

	private CPUFlag GetFlag(int opcode)
	{
		return opcode switch
		{
			0x20 => CPUFlag.ZERO,
			0x28 => CPUFlag.ZERO,
			0x30 => CPUFlag.CARRY,
			0x38 => CPUFlag.CARRY,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	private bool ExpectedValue(int opcode)
	{
		return opcode switch
		{
			0x20 => false,
			0x28 => true,
			0x30 => false,
			0x38 => true,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var offset = (sbyte)memory.ReadByte((ushort)(addr + 1));
		var negativePrefix = ExpectedValue(opcode) ? "" : "N";
		var flagChar = GetFlag(opcode).GetChar();
		return base.ToString() + $" ${negativePrefix}${flagChar}, ${offset}";
	}
}