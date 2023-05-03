namespace GBMU.Core;

public class OperatorRET : CPUOperator
{
	public OperatorRET() : base("RET", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		cpu.PC = cpu.PopFromStack();
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString();
}