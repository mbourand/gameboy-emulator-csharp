namespace GBMU.Core;

public class OperatorJP : CPUOperator
{
	private OperationDataType _destinationDataType;

	public OperatorJP(OperationDataType offsetDataType) : base("JP", 1)
	{
		_destinationDataType = offsetDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		cpu.PC = _destinationDataType.GetSourceValue(cpu, memory);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString() + $" ${_destinationDataType.GetMnemonic()}";
}