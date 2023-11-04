using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorSWAP : CPUOperator {
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorSWAP(OperationDataType destinationDataType, FlagPermissionHandler flagPermissionHandler) : base("SWAP", 1) {
		_destinationDataType = destinationDataType;
		_flagPermissionHandler = flagPermissionHandler;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		ushort source = _destinationDataType.GetSourceValue(cpu, memory);

		byte result = (byte)(((source & 0b0000_1111) << 4) | ((source & 0b1111_0000) >> 4));

		var flags = new Dictionary<CPUFlag, bool>()
		{
			{ CPUFlag.Zero, result == 0 },
		};

		_flagPermissionHandler.Apply(cpu, flags);
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}";
}