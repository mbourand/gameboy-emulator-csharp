using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorADD : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;
	private FlagPermissionHandler _flagHandler;
	private CarryBit _carryBit;
	private HalfCarryBit _halfCarryBit;
	private bool _isSourceSigned;

	public OperatorADD(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler, CarryBit carryBit, HalfCarryBit halfCarryBit, bool isSourceSigned = false)
		: base("ADD", 1)
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

		var result = (ushort)(signedSource + destinationValue);
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	private void ApplyFlags(CPU cpu, int a, int b)
	{
		var halfCarryMask = (int)_halfCarryBit - 1;

		int result = a + b;
		int halfResult = (a & halfCarryMask) + (b & halfCarryMask);

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.N_SUBTRACT, false },
			{ CPUFlag.ZERO, (byte)result == 0x00 },
			{ CPUFlag.CARRY, result >= (int)_carryBit },
			{ CPUFlag.HALF_CARRY, halfResult > halfCarryMask },
		};

		_flagHandler.Apply(cpu, newFlags);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" ${_destinationDataType.GetMnemonic()}, ${_sourceDataType.GetMnemonic()}";
	}
}