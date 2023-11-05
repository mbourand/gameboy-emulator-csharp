namespace GBMU.Core;

public class OperatorHALT : CPUOperator {
	public OperatorHALT() : base("HALT", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.Halted = true;
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}