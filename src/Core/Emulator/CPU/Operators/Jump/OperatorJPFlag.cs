namespace GBMU.Core;

public class OperatorJPFlag : CPUOperator {
	private readonly CPUFlag _flag;
	private readonly bool _expectedValue;
	private readonly OperationDataType _destinationDataType;

	private bool _branched;

	public OperatorJPFlag(CPUFlag flag, bool expectedValue, OperationDataType destinationDataType) : base("JP", 1) {
		_flag = flag;
		_expectedValue = expectedValue;
		_destinationDataType = destinationDataType;
		_branched = false;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		_branched = cpu.GetFlag(_flag) == _expectedValue;
		if (_branched)
			cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) {
		if (!_branched)
			base.ShiftPC(cpu, memory);
	}

	public override int GetCycles(bool cbPrefix, byte opcode) => _branched ? 16 : 12;

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {FlagUtils.FlagConditionToMnemonic(_flag, _expectedValue)}, ${_destinationDataType.GetMnemonic()}";
	}
}