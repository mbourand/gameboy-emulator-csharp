namespace GBMU.Core;

public class OperatorPOP : CPUOperator {
	private readonly OperationDataType _destinationDataType;

	public OperatorPOP(OperationDataType destinationDataType) : base("POP", 1) {
		_destinationDataType = destinationDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var value = cpu.PopFromStack();
		_destinationDataType.WriteToDestination(cpu, memory, value);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}";
	}
}