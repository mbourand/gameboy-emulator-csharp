namespace GBMU.Core;

public class OperatorADDReg16Reg16 : CPUOperator
{
	public OperatorADDReg16Reg16() : base("ADD", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var srcValue = cpu.Get16BitRegister(GetSourceRegister(opcode));

		var dstRegister = GetDestinationRegister(opcode);
		var dstValue = cpu.Get16BitRegister(dstRegister);

		int result = srcValue + dstValue;

		cpu.SetFlag(CPUFlag.N_SUBTRACT, false);
		cpu.SetFlag(CPUFlag.CARRY, result > 0xFFFF);
		cpu.SetFlag(CPUFlag.HALF_CARRY, ((srcValue & 0xFFF) + (dstValue & 0xFFF)) > 0xFFF);

		cpu.Set16BitRegister(dstRegister, (ushort)result);

		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetDestinationRegister(int opcode)
	{
		return opcode switch
		{
			0x09 => CPURegister.HL,
			0x19 => CPURegister.HL,
			0x29 => CPURegister.HL,
			0x39 => CPURegister.HL,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	private CPURegister GetSourceRegister(int opcode)
	{
		return opcode switch
		{
			0x09 => CPURegister.BC,
			0x19 => CPURegister.DE,
			0x29 => CPURegister.HL,
			0x39 => CPURegister.SP,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var destination = GetDestinationRegister(opcode).ToString();
		var source = GetSourceRegister(opcode).ToString();
		return base.ToString() + $" ${destination}, ${source}";
	}
}