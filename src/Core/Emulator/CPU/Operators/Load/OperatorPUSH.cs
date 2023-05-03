namespace GBMU.Core;

public class OperatorPUSH : CPUOperator
{
	private OperationDataType _sourceDataType;

	public OperatorPUSH(OperationDataType sourceDataType) : base("PUSH", 1)
	{
		_sourceDataType = sourceDataType;
		length += _sourceDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var value = _sourceDataType.GetSourceValue(cpu, memory);
		cpu.PushToStack(value);
		base.Execute(cpu, memory, opcode);
	}
}