using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSUB : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;
	private FlagHandler _flagHandler;
	private CarryBit _carryBit;
	private HalfCarryBit _halfCarryBit;
	private bool _isSourceSigned;

	public OperatorSUB(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagHandler flagHandler, CarryBit carryBit, HalfCarryBit halfCarryBit, bool isSourceSigned = false)
		: base("SUB", 1)
	{
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_carryBit = carryBit;
		_halfCarryBit = halfCarryBit;
		_flagHandler = flagHandler;
		_isSourceSigned = isSourceSigned;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);
		int signedSource = _isSourceSigned ? (sbyte)((byte)sourceValue) : sourceValue;

		ApplyFlags(cpu, signedSource, destinationValue);

		var result = (ushort)(destinationValue - signedSource);
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	private void ApplyFlags(CPU cpu, int source, int destination)
	{
		var halfCarryMask = (int)_halfCarryBit - 1;

		int result = destination - source;
		int halfResult = (source & halfCarryMask) - (destination & halfCarryMask);

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.N_SUBTRACT, true },
			{ CPUFlag.ZERO, (byte)result == 0x00 },
			{ CPUFlag.CARRY, result < 0 },
			{ CPUFlag.HALF_CARRY, halfResult < 0 },
		};

		_flagHandler.Apply(cpu, newFlags);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" ${_destinationDataType.GetMnemonic()}, ${_sourceDataType.GetMnemonic()}";
	}
}