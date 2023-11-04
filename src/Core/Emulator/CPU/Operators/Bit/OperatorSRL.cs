using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSRL : CPUOperator {
	private readonly OperationDataType _dataType;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorSRL(OperationDataType dataType, FlagPermissionHandler flagPermissionHandler) : base("SRL", 1) {
		_dataType = dataType;
		_flagPermissionHandler = flagPermissionHandler;
		length += dataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _dataType.GetSourceValue(cpu, memory);

		var result = (byte)(sourceValue >> 1);

		var flags = new Dictionary<CPUFlag, bool>()
		{
			{ CPUFlag.Zero, result == 0 },
			{ CPUFlag.Carry, (sourceValue & 0b0000_0001) != 0 },
		};
		_flagPermissionHandler.Apply(cpu, flags);

		_dataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" {_dataType.GetMnemonic()}";
}