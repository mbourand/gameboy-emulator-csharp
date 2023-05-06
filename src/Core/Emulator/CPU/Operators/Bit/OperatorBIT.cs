using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorBIT : CPUOperator
{
	private OperationDataType _sourceDataType;
	private byte _index;
	private FlagPermissionHandler _flagPermissionHandler;

	public OperatorBIT(OperationDataType sourceDataType, byte index, FlagPermissionHandler flagPermissionHandler) : base("BIT", 1)
	{
		_sourceDataType = sourceDataType;
		_index = index;
		_flagPermissionHandler = flagPermissionHandler;
		length += (byte)(_sourceDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var bitValue = (sourceValue >> _index) & 0b1;

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.ZERO, bitValue == 0 },
			{ CPUFlag.N_SUBTRACT, false },
			{ CPUFlag.HALF_CARRY, true },
		};

		_flagPermissionHandler.Apply(cpu, newFlags);

		base.Execute(cpu, memory, opcode);
	}
}