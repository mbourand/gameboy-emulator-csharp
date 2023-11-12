using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace GBMU.Core;

public class OperatorAND : CPUOperator {
	private readonly OperationDataType _sourceDataType;
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagHandler;

	public OperatorAND(OperationDataType sourceDataType, OperationDataType destinationDataType, FlagPermissionHandler flagHandler) : base("AND", 1) {
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		_flagHandler = flagHandler;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _sourceDataType.GetSourceValue(cpu, memory);
		var destinationValue = _destinationDataType.GetSourceValue(cpu, memory);

		ushort result = (ushort)(sourceValue & destinationValue);
		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{CPUFlag.Zero, result == 0},
			{CPUFlag.NSubtract, false},
			{CPUFlag.HalfCarry, true},
			{CPUFlag.Carry, false},
		};
		_flagHandler.Apply(cpu, newFlags);

		_destinationDataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}, {_sourceDataType.GetMnemonic()}";
	}
}