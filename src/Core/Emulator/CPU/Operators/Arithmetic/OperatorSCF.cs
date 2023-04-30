namespace GBMU.Core;

public class OperatorSCF : CPUOperator
{
	public OperatorSCF() : base("SCF", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		cpu.SetFlag(CPUFlag.N_SUBTRACT, false);
		cpu.SetFlag(CPUFlag.HALF_CARRY, false);
		cpu.SetFlag(CPUFlag.CARRY, true);
		base.Execute(cpu, memory, opcode);
	}
}