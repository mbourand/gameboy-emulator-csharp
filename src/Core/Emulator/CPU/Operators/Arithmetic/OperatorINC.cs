using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorINC : CPUOperator {
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorINC(OperationDataType destinationDataType, FlagPermissionHandler flagPermissionHandler) : base("INC", 1) {
		_destinationDataType = destinationDataType;
		_flagPermissionHandler = flagPermissionHandler;

		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var value = _destinationDataType.GetSourceValue(cpu, memory);

		var newFlags = new Dictionary<CPUFlag, bool>() {
			{CPUFlag.NSubtract, false},
			{CPUFlag.HalfCarry, (value & 0x0F) == 0x0F},
			{CPUFlag.Zero, value == 0xFF}
		};
		_flagPermissionHandler.Apply(cpu, newFlags);

		_destinationDataType.WriteToDestination(cpu, memory, (ushort)(value + 1));
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}";
	}
}