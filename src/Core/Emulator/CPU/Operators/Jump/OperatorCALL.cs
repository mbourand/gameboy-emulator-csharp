namespace GBMU.Core;

public class OperatorCALL : CPUOperator {
	private readonly OperationDataType _destinationDataType;

	public OperatorCALL(OperationDataType destinationDataType) : base("CALL", 1) {
		_destinationDataType = destinationDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.PushToStack((ushort)(cpu.PC + length));
		cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) {
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" ${_destinationDataType.GetMnemonic()}";
}