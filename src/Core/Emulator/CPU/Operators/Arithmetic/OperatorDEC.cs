namespace GBMU.Core;

public class OperatorDEC : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;

	public OperatorDEC(OperationDataType sourceDataType, OperationDataType destinationDataType) : base("DEC", 1)
	{
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;
		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var value = _sourceDataType.GetSourceValue(cpu, memory);

		cpu.SetFlag(CPUFlag.N_SUBTRACT, true);
		cpu.SetFlag(CPUFlag.HALF_CARRY, (value & 0x0F) == 0x00);
		cpu.SetFlag(CPUFlag.ZERO, value == 0x01);

		_destinationDataType.WriteToDestination(cpu, memory, (byte)(value - 1));
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" {_destinationDataType.GetMnemonic()}, {_sourceDataType.GetMnemonic()}";
	}
}