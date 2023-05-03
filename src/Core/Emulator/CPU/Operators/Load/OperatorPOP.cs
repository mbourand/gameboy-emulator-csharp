namespace GBMU.Core;

public class OperatorPOP : CPUOperator
{
	private OperationDataType _destinationDataType;

	public OperatorPOP(OperationDataType destinationDataType) : base("POP", 1)
	{
		_destinationDataType = destinationDataType;
		length += _destinationDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var value = cpu.PopFromStack();
		_destinationDataType.WriteToDestination(cpu, memory, value);
		base.Execute(cpu, memory, opcode);
	}
}