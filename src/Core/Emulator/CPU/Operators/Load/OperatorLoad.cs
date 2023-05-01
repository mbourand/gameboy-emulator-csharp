using System;

namespace GBMU.Core;

// Cette classe effectue une opération de chargement de données selon le type de données source et de destination
public class OperatorLoad : CPUOperator
{
	private OperationDataType _sourceDataType;
	private OperationDataType _destinationDataType;

	public OperatorLoad(OperationDataType sourceDataType, OperationDataType destinationDataType) : base("LD", 1)
	{
		_sourceDataType = sourceDataType;
		_destinationDataType = destinationDataType;

		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var source = _sourceDataType.GetSourceValue(cpu, memory);
		_destinationDataType.WriteToDestination(cpu, memory, source);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}, {_sourceDataType.GetMnemonic()}";
	}
}