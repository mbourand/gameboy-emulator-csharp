namespace GBMU.Core;

public class OperatorDAA : CPUOperator {
	public OperatorDAA() : base("DAA", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		bool carry = cpu.GetFlag(CPUFlag.Carry);
		bool halfCarry = cpu.GetFlag(CPUFlag.HalfCarry);

		if (cpu.GetFlag(CPUFlag.NSubtract)) {
			if (carry)
				cpu.A -= 0x60;
			if (halfCarry)
				cpu.A -= 0x6;
		} else {
			if (carry || (cpu.A > 0x99)) {
				cpu.A += 0x60;
				cpu.SetFlag(CPUFlag.Carry, true);
			}
			if (halfCarry || (cpu.A & 0xF) > 0x9)
				cpu.A += 0x6;
		}

		cpu.SetFlag(CPUFlag.Zero, cpu.A == 0);
		cpu.SetFlag(CPUFlag.HalfCarry, false);
		base.Execute(cpu, memory, opcode);
	}
}