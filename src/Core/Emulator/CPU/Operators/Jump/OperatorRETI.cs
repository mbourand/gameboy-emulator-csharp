namespace GBMU.Core;

public class OperatorRETI : CPUOperator {
	public OperatorRETI() : base("RETI", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.PC = cpu.PopFromStack();
		cpu.SetInterruptMasterEnable(true);
		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) {
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}