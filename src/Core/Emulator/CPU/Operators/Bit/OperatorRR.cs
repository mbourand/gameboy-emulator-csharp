using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorRR : CPUOperator {
	private readonly OperationDataType _dataType;
	private readonly FlagPermissionHandler _flagPermissionHandler;

	public OperatorRR(OperationDataType dataType, FlagPermissionHandler flagPermissionHandler) : base("RR", 1) {
		_dataType = dataType;
		_flagPermissionHandler = flagPermissionHandler;
		length += dataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		bool wasCarrySet = cpu.GetFlag(CPUFlag.Carry);
		var sourceValue = _dataType.GetSourceValue(cpu, memory);

		var result = (byte)((sourceValue >> 1) | (wasCarrySet ? 0b1000_0000 : 0));

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.Zero, result == 0 },
			{ CPUFlag.NSubtract, false },
			{ CPUFlag.HalfCarry, false },
			{ CPUFlag.Carry, (sourceValue & 0b0000_0001) != 0 },
		};
		_flagPermissionHandler.Apply(cpu, newFlags);

		_dataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}
}