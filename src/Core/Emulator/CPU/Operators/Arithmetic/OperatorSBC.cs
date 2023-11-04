using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSBC : CPUOperator {
	private readonly OperationDataType _sourceDataType;
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagHandler;
	private readonly CarryBit _carryBit;
	private readonly HalfCarryBit _halfCarryBit;

	public OperatorSBC(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler, CarryBit carryBit, HalfCarryBit halfCarryBit)
		: base("SBC", 1) {
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_carryBit = carryBit;
		_halfCarryBit = halfCarryBit;
		_flagHandler = flagHandler;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);

		ApplyFlags(cpu, sourceValue, destinationValue);

		var result = (ushort)(destinationValue - sourceValue - (cpu.GetFlag(CPUFlag.Carry) ? 1 : 0));
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	private void ApplyFlags(CPU cpu, int sourceValue, int destinationValue) {
		var halfCarryMask = (int)_halfCarryBit - 1;

		var result = (ushort)(destinationValue - sourceValue - (cpu.GetFlag(CPUFlag.Carry) ? 1 : 0));
		int halfResult = (destinationValue & halfCarryMask) - (sourceValue & halfCarryMask) - (cpu.GetFlag(CPUFlag.Carry) ? 1 : 0);

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