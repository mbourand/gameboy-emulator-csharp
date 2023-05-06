using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorRES : CPUOperator
{
	private OperationDataType _dataType;
	private byte _index;

	public OperatorRES(OperationDataType dataType, byte index) : base("RES", 1)
	{
		_dataType = dataType;
		_index = index;
		length += (byte)(dataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = _dataType.GetSourceValue(cpu, memory);

		var resMask = (byte)(~(1 << _index));
		var result = (byte)(sourceValue & resMask);

		_dataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}
}