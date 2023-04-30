namespace GBMU.Core;

public class OperatorNOP : CPUOperator
{
	public OperatorNOP() : base("NOP", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString();
}