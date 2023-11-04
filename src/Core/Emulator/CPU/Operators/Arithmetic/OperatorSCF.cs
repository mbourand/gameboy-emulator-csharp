namespace GBMU.Core;

public class OperatorSCF : CPUOperator {
	public OperatorSCF() : base("SCF", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.SetFlag(CPUFlag.NSubtract, false);
		cpu.SetFlag(CPUFlag.HalfCarry, false);
		cpu.SetFlag(CPUFlag.Carry, true);
		base.Execute(cpu, memory, opcode);
	}
}