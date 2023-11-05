namespace GBMU.Core;

public class OperatorRLA : CPUOperator {
	private readonly OperationDataType _dataType;

	public OperatorRLA() : base("RLA", 1) {
		_dataType = new DataTypeReg8(CPURegister.A);
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		bool wasCarrySet = cpu.GetFlag(CPUFlag.Carry);
		var sourceValue = _dataType.GetSourceValue(cpu, memory);
		bool willOverflow = (sourceValue & 0b1000_0000) != 0;
		cpu.ResetFlags();
		cpu.SetFlag(CPUFlag.Carry, willOverflow);
		cpu.A = (byte)((cpu.A << 1) | (wasCarrySet ? 1 : 0));

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {_dataType.GetMnemonic()}";
	}
}