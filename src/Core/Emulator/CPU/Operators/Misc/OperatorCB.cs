namespace GBMU.Core;

public class OperatorCB : CPUOperator {
	public OperatorCB() : base("CB", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.CBPrefix = true;
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}