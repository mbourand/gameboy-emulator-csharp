namespace GBMU.Core;

public class OperatorCPL : CPUOperator {
	public OperatorCPL() : base("CPL", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.A = (byte)~cpu.A;

		cpu.SetFlag(CPUFlag.HalfCarry, true);
		cpu.SetFlag(CPUFlag.NSubtract, true);

		base.Execute(cpu, memory, opcode);
	}
}