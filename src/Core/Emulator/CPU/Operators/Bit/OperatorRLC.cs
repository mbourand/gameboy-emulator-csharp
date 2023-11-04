using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorRLC : CPUOperator {
	private readonly OperationDataType _destinationDataType;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorRLC(OperationDataType destinationDataType, FlagPermissionHandler flagPermissionHandler) : base("RLC", 1) {
		_destinationDataType = destinationDataType;
		_flagPermissionHandler = flagPermissionHandler;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		ushort source = _destinationDataType.GetSourceValue(cpu, memory);
		bool willOverflow = (source & 0b1000_0000) != 0;
		byte result = (byte)((source << 1) | (source >> 7));

		var flags = new Dictionary<CPUFlag, bool>()
		{
			{ CPUFlag.Zero, result == 0 },
			{ CPUFlag.Carry, willOverflow },
		};

		_flagPermissionHandler.Apply(cpu, flags);
		_destinationDataType.WriteToDestination(cpu, memory, result);

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}";
}