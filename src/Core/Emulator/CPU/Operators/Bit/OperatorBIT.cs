using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorBIT : CPUOperator {
	private readonly OperationDataType _sourceDataType;
	private readonly byte _index;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorBIT(OperationDataType sourceDataType, byte index, FlagPermissionHandler flagPermissionHandler) : base("BIT", 1) {
		_sourceDataType = sourceDataType;
		_index = index;
		_flagPermissionHandler = flagPermissionHandler;
		length += _sourceDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var bitValue = (sourceValue >> _index) & 0b1;

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.Zero, bitValue == 0 },
			{ CPUFlag.NSubtract, false },
			{ CPUFlag.HalfCarry, true },
		};

		_flagPermissionHandler.Apply(cpu, newFlags);

		base.Execute(cpu, memory, opcode);
	}
}