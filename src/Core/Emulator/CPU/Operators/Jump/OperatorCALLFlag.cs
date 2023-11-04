namespace GBMU.Core;

public class OperatorCALLFlag : CPUOperator {
	private readonly CPUFlag _flag;
	private readonly bool _expectedValue;
	private readonly OperationDataType _destinationDataType;

	private bool _branched;

	public OperatorCALLFlag(CPUFlag flag, bool expectedValue, OperationDataType destinationDataType) : base("CALL", 1) {
		_flag = flag;
		_expectedValue = expectedValue;
		_destinationDataType = destinationDataType;
		_branched = false;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		_branched = cpu.GetFlag(_flag) == _expectedValue;
		if (_branched) {
			cpu.PushToStack((ushort)(cpu.PC + length));
			cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		}
		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) {
		if (!_branched)
			base.ShiftPC(cpu, memory);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" ${FlagUtils.FlagConditionToMnemonic(_flag, _expectedValue)}, ${_destinationDataType.GetMnemonic()}";
	}
}