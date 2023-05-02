namespace GBMU.Core;

public class OperatorCCF : CPUOperator
{
	public OperatorCCF() : base("CCF", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		cpu.A = (byte)(~cpu.A);

		cpu.SetFlag(CPUFlag.N_SUBTRACT, false);
		cpu.SetFlag(CPUFlag.HALF_CARRY, false);
		cpu.SetFlag(CPUFlag.CARRY, !cpu.GetFlag(CPUFlag.CARRY));

		base.Execute(cpu, memory, opcode);
	}
}