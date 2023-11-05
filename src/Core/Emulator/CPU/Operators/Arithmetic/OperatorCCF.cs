namespace GBMU.Core;

public class OperatorCCF : CPUOperator {
	public OperatorCCF() : base("CCF", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.SetFlag(CPUFlag.NSubtract, false);
		cpu.SetFlag(CPUFlag.HalfCarry, false);
		cpu.SetFlag(CPUFlag.Carry, !cpu.GetFlag(CPUFlag.Carry));

		base.Execute(cpu, memory, opcode);
	}
}