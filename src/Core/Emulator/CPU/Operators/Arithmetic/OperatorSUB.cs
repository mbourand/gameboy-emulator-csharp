using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSUB : CPUOperator {
	private readonly OperationDataType _sourceDataType;
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagHandler;
	private readonly CarryBit _carryBit;
	private readonly HalfCarryBit _halfCarryBit;
	private readonly bool _isSourceSigned;

	public OperatorSUB(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler, CarryBit carryBit, HalfCarryBit halfCarryBit, bool isSourceSigned = false)
		: base("SUB", 1) {
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_carryBit = carryBit;
		_halfCarryBit = halfCarryBit;
		_flagHandler = flagHandler;
		_isSourceSigned = isSourceSigned;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);
		int signedSource = _isSourceSigned ? (sbyte)(byte)sourceValue : sourceValue;

		ApplyFlags(cpu, signedSource, destinationValue);

		var result = (ushort)(destinationValue - signedSource);
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	private void ApplyFlags(CPU cpu, int source, int destination) {
		var halfCarryMask = (int)_halfCarryBit - 1;
		var carryMask = (int)_carryBit - 1;

		int result = (destination & carryMask) - (source & carryMask);
		int halfResult = (destination & halfCarryMask) - (source & halfCarryMask);

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.NSubtract, true },
			{ CPUFlag.Zero, (byte)result == 0x00 },
			{ CPUFlag.Carry, result < 0 },
			{ CPUFlag.HalfCarry, halfResult < 0 },
		};

		_flagHandler.Apply(cpu, newFlags);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" ${_destinationDataType.GetMnemonic()}, ${_sourceDataType.GetMnemonic()}";
	}
}