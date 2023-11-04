namespace GBMU.Core;

public class OperatorDI : CPUOperator {

	public OperatorDI() : base("DI", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.SetInterruptMasterEnable(false);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}