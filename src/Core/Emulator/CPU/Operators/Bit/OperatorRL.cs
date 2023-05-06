using System.Collections.Generic;

namespace GBMU.Core;

public class OperatorRL : CPUOperator
{
	private OperationDataType _dataType;
	private FlagPermissionHandler _flagPermissionHandler;

	public OperatorRL(OperationDataType dataType, FlagPermissionHandler flagPermissionHandler) : base("RL", 1)
	{
		_dataType = dataType;
		_flagPermissionHandler = flagPermissionHandler;
		length += (byte)(dataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		bool wasCarrySet = cpu.GetFlag(CPUFlag.CARRY);
		var sourceValue = _dataType.GetSourceValue(cpu, memory);

		var result = (byte)((sourceValue << 1) | (wasCarrySet ? 1 : 0));

		Dictionary<CPUFlag, bool> newFlags = new()
		{
			{ CPUFlag.ZERO, result == 0 },
			{ CPUFlag.N_SUBTRACT, false },
			{ CPUFlag.HALF_CARRY, false },
			{ CPUFlag.CARRY, (sourceValue & 0b1000_0000) != 0 },
		};
		_flagPermissionHandler.Apply(cpu, newFlags);

		_dataType.WriteToDestination(cpu, memory, result);
		base.Execute(cpu, memory, opcode);
	}
}