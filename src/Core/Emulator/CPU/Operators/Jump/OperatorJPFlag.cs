namespace GBMU.Core;

public class OperatorJPFlag : CPUOperator
{
	private CPUFlag _flag;
	private bool _expectedValue;
	private OperationDataType _destinationDataType;

	public OperatorJPFlag(CPUFlag flag, bool expectedValue, OperationDataType destinationDataType) : base("JP", 1)
	{
		_flag = flag;
		_expectedValue = expectedValue;
		_destinationDataType = destinationDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		if (cpu.GetFlag(_flag) == _expectedValue)
			cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var negativePrefix = _expectedValue ? "" : "N";
		var flagChar = _flag.GetChar();
		return base.ToString() + $" ${negativePrefix}${flagChar}, ${_destinationDataType.GetMnemonic()}";
	}
}