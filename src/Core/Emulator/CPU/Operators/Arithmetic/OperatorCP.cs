using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorCP : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;
	private FlagPermissionHandler _flagHandler;

	public OperatorCP(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler) : base("CP", 1)
	{
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_flagHandler = flagHandler;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);

		int result = destinationValue - sourceValue;
		int halfResult = (destinationValue & 0xF) - (sourceValue & 0xF);
		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{CPUFlag.ZERO, result == 0},
			{CPUFlag.N_SUBTRACT, true},
			{ CPUFlag.CARRY, result < 0 },
			{ CPUFlag.HALF_CARRY, halfResult < 0 },
		};
		_flagHandler.Apply(cpu, newFlags);

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" {_destinationDataType.GetMnemonic()}, {_sourceDataType.GetMnemonic()}";
	}
}