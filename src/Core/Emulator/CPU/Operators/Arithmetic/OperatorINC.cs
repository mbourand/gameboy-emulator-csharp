namespace GBMU.Core;

public class OperatorINC : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;

	public OperatorINC(OperationDataType sourceDataType, OperationDataType destinationDataType) : base("INC", 1)
	{
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var value = _sourceDataType.GetSourceValue(cpu, memory);

		cpu.SetFlag(CPUFlag.N_SUBTRACT, false);
		cpu.SetFlag(CPUFlag.HALF_CARRY, (value & 0x0F) == 0x0F);
		cpu.SetFlag(CPUFlag.ZERO, value == 0xFF);

		_destinationDataType.WriteToDestination(cpu, memory, (byte)(value + 1));
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" {_destinationDataType.GetMnemonic()}, {_sourceDataType.GetMnemonic()}";
	}
}