namespace GBMU.Core;

public class OperatorEI : CPUOperator {

	public OperatorEI() : base("EI", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.SetInterruptMasterEnable(true);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}