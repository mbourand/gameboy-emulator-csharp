namespace GBMU.Core;

public enum ShiftingBehaviour {
	Increment = 1,
	Decrement = -1,
}

// Cette classe effectue une opération de chargement de données selon le type de données source et de destination
public class OperatorLDShift : OperatorLD {

	private readonly ShiftingBehaviour _shiftingBehaviour;
	private readonly CPURegister _registerToShift;

	public OperatorLDShift(OperationDataType sourceDataType, OperationDataType destinationDataType, CPURegister registerToShift, ShiftingBehaviour shiftingBehaviour)
		: base(sourceDataType, destinationDataType) {
		_shiftingBehaviour = shiftingBehaviour;
		_registerToShift = registerToShift;
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		base.Execute(cpu, memory, opcode);
		ushort shiftedValue = (ushort)(cpu.Get16BitRegister(_registerToShift) + (int)_shiftingBehaviour);
		cpu.Set16BitRegister(_registerToShift, shiftedValue);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		string shiftingOperator = _shiftingBehaviour == ShiftingBehaviour.Increment ? "+" : "-";
		return base.ToString(cpu, memory, opcode, addr) + $" / {_registerToShift.ToString()}{shiftingOperator}";
	}
}