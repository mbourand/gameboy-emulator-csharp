namespace GBMU.Core;

public class OperatorPUSH : CPUOperator {
	private readonly OperationDataType _sourceDataType;

	public OperatorPUSH(OperationDataType sourceDataType) : base("PUSH", 1) {
		_sourceDataType = sourceDataType;
		length += _sourceDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var value = _sourceDataType.GetSourceValue(cpu, memory);
		cpu.PushToStack(value);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {_sourceDataType.GetMnemonic()}";
	}
}