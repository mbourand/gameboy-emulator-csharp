using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorADC : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;
	private FlagPermissionHandler _flagHandler;
	private CarryBit _carryBit;
	private HalfCarryBit _halfCarryBit;

	public OperatorADC(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler, CarryBit carryBit, HalfCarryBit halfCarryBit)
		: base("ADC", 1)
	{
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_carryBit = carryBit;
		_halfCarryBit = halfCarryBit;
		_flagHandler = flagHandler;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);

		ApplyFlags(cpu, sourceValue, destinationValue);

		var result = (ushort)(sourceValue + destinationValue + (cpu.GetFlag(CPUFlag.CARRY) ? 1 : 0));
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	private void ApplyFlags(CPU cpu, int sourceValue, int destinationValue)
	{
		var halfCarryMask = (int)_halfCarryBit - 1;

		int result = destinationValue + sourceValue + (cpu.GetFlag(CPUFlag.CARRY) ? 1 : 0);
		int halfResult = (destinationValue & halfCarryMask) + (sourceValue & halfCarryMask) + (cpu.GetFlag(CPUFlag.CARRY) ? 1 : 0);

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