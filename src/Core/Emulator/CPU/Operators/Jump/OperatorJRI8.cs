namespace GBMU.Core;

public class OperatorJRI8 : CPUOperator
{
	public OperatorJRI8() : base("ADD", 2) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var offset = (sbyte)memory.ReadByte((ushort)(cpu.PC + 1));
		cpu.PC = (ushort)(cpu.PC + offset);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var offset = (sbyte)memory.ReadByte((ushort)(addr + 1));
		return base.ToString() + $" ${offset}";
	}
}