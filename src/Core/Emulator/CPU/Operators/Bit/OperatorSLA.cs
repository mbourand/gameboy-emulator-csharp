using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSLA : CPUOperator {
	private readonly OperationDataType _dataType;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorSLA(OperationDataType dataType, FlagPermissionHandler flagPermissionHandler) : base("SLA", 1) {
		_dataType = dataType;
		_flagPermissionHandler = flagPermissionHandler;
		length += dataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var sourceValue = _dataType.GetSourceValue(cpu, memory);

		var result = (byte)(sourceValue << 1);
		var willOverflow = (sourceValue & 0b1000_0000) != 0;

		var flags = new Dictionary<CPUFlag, bool>()
		{
			{ CPUFlag.Zero, result == 0 },
			{ CPUFlag.Carry, willOverflow },
		};
		_flagPermissionHandler.Apply(cpu, flags);

		_dataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" {_dataType.GetMnemonic()}";
}