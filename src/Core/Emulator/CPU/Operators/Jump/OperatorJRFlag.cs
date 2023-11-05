namespace GBMU.Core;

public class OperatorJRFlag : CPUOperator {
	private readonly CPUFlag _flag;
	private readonly bool _expectedValue;
	private readonly OperationDataType _offsetDataType;
	private bool _branched;

	public OperatorJRFlag(CPUFlag flag, bool expectedValue, OperationDataType offsetDataType) : base("JR", 1) {
		_flag = flag;
		_expectedValue = expectedValue;
		_offsetDataType = offsetDataType;
		_branched = false;

		length += _offsetDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		_branched = cpu.GetFlag(_flag) == _expectedValue;
		if (_branched) {
			var offset = (sbyte)(byte)_offsetDataType.GetSourceValue(cpu, memory);
			cpu.PC = (ushort)(cpu.PC + offset);
		}

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {FlagUtils.FlagConditionToMnemonic(_flag, _expectedValue)}, ${_offsetDataType.GetMnemonic()}";
	}
}