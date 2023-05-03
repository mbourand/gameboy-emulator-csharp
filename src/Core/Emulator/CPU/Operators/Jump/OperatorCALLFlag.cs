namespace GBMU.Core;

public class OperatorCALLFlag : CPUOperator
{
	private CPUFlag _flag;
	private bool _expectedValue;
	private OperationDataType _destinationDataType;

	public OperatorCALLFlag(CPUFlag flag, bool expectedValue, OperationDataType destinationDataType) : base("CALL", 1)
	{
		_flag = flag;
		_expectedValue = expectedValue;
		_destinationDataType = destinationDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		if (cpu.GetFlag(_flag) == _expectedValue)
		{
			cpu.PushToStack((ushort)(cpu.PC + length));
			cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		}
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" ${FlagUtils.FlagConditionToMnemonic(_flag, _expectedValue)}, ${_destinationDataType.GetMnemonic()}";
	}
}