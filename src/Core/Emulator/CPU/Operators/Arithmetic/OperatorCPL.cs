namespace GBMU.Core;

public class OperatorCPL : CPUOperator
{
	public OperatorCPL() : base("CPL", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		cpu.A = (byte)(~cpu.A);

		cpu.SetFlag(CPUFlag.HALF_CARRY, true);
		cpu.SetFlag(CPUFlag.N_SUBTRACT, true);

		base.Execute(cpu, memory, opcode);
	}
}