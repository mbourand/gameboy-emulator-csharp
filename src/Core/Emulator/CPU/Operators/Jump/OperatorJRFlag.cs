namespace GBMU.Core;

public class OperatorJRFlag : CPUOperator
{
	private CPUFlag _flag;
	private bool _expectedValue;
	private OperationDataType _offsetDataType;

	public OperatorJRFlag(CPUFlag flag, bool expectedValue, OperationDataType offsetDataType) : base("JR", 1)
	{
		_flag = flag;
		_expectedValue = expectedValue;
		_offsetDataType = offsetDataType;
		length += _offsetDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		if (cpu.GetFlag(_flag) == _expectedValue)
		{
			var offset = ((sbyte)((byte)_offsetDataType.GetSourceValue(cpu, memory)));
			cpu.PC = (ushort)(cpu.PC - offset);
		}

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var negativePrefix = _expectedValue ? "" : "N";
		var flagChar = _flag.GetChar();
		return base.ToString() + $" ${negativePrefix}${flagChar}, ${_offsetDataType.GetMnemonic()}";
	}
}