namespace GBMU.Core;

public class OperatorSTOP : CPUOperator {
	public OperatorSTOP() : base("STOP", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.Stopped = true; // TODO: Use real STOP behavior
		memory.WriteByte(Memory.DIV.Address, 0);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}