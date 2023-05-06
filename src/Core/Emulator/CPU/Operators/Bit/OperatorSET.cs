namespace GBMU.Core;

public class OperatorSET : CPUOperator
{
	private OperationDataType _dataType;
	private byte _index;

	public OperatorSET(OperationDataType dataType, byte index) : base("SET", 1)
	{
		_dataType = dataType;
		_index = index;
		length += (byte)(dataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = _dataType.GetSourceValue(cpu, memory);

		var resMask = (byte)(1 << _index);
		var result = (byte)(sourceValue | resMask);

		_dataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}
}