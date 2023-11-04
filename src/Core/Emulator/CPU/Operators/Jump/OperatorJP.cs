namespace GBMU.Core;

public class OperatorJP : CPUOperator {
	private readonly OperationDataType _destinationDataType;

	public OperatorJP(OperationDataType destinationDataType) : base("JP", 1) {
		_destinationDataType = destinationDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) {
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" ${_destinationDataType.GetMnemonic()}";
}