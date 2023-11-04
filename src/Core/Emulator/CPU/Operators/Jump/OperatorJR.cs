namespace GBMU.Core;

public class OperatorJR : CPUOperator {
	private readonly OperationDataType _offsetDataType;

	public OperatorJR(OperationDataType offsetDataType) : base("JR", 1) {
		_offsetDataType = offsetDataType;
		length += _offsetDataType.GetLength();
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		var offset = (sbyte)(byte)_offsetDataType.GetSourceValue(cpu, memory);
		cpu.PC = (ushort)(cpu.PC + offset);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr) + $" {_offsetDataType.GetMnemonic()}";
}