namespace GBMU.Core;

public class OperatorRRA : CPUOperator {
	public OperatorRRA() : base("RRA", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		bool wasCarrySet = cpu.GetFlag(CPUFlag.Carry);
		bool willOverflow = (cpu.A & 0b0000_0001) != 0;
		cpu.ResetFlags();
		cpu.SetFlag(CPUFlag.Carry, willOverflow);
		cpu.A = (byte)((cpu.A >> 1) | (wasCarrySet ? 0b1000_0000 : 0));

		base.Execute(cpu, memory, opcode);
	}
}