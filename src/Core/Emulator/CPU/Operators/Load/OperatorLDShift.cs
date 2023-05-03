using System;

namespace GBMU.Core;

public enum ShiftingBehaviour
{
	INCREMENT = 1,
	DECREMENT = -1,
}

// Cette classe effectue une opération de chargement de données selon le type de données source et de destination
public class OperatorLDShift : OperatorLD
{

	private ShiftingBehaviour _shiftingBehaviour;
	private CPURegister _registerToShift;

	public OperatorLDShift(OperationDataType sourceDataType, OperationDataType destinationDataType, CPURegister registerToShift, ShiftingBehaviour shiftingBehaviour)
		: base(sourceDataType, destinationDataType)
	{
		this._shiftingBehaviour = shiftingBehaviour;
		this._registerToShift = registerToShift;
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		base.Execute(cpu, memory, opcode);
		ushort shiftedValue = (ushort)(cpu.Get16BitRegister(_registerToShift) + (int)_shiftingBehaviour);
		cpu.Set16BitRegister(_registerToShift, shiftedValue);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		string shiftingOperator = _shiftingBehaviour == ShiftingBehaviour.INCREMENT ? "+" : "-";
		return base.ToString(cpu, memory, opcode, addr) + $" / {_registerToShift.ToString()}{shiftingOperator}";
	}
}