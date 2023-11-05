using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSBC : CPUOperator {
	private readonly OperationDataType _sourceDataType;
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagHandler;

	public OperatorSBC(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler)
		: base("SBC", 1) {
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_flagHandler = flagHandler;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);
		int carryValue = cpu.GetFlag(CPUFlag.Carry) ? 1 : 0;

		ApplyFlags(cpu, sourceValue, destinationValue);

		var result = (ushort)(destinationValue - sourceValue - carryValue);
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	private void ApplyFlags(CPU cpu, int sourceValue, int destinationValue) {
		var halfCarryMask = (int)HalfCarryBit.Bit4 - 1;

		int result = destinationValue - sourceValue - (cpu.GetFlag(CPUFlag.Carry) ? 1 : 0);
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