namespace GBMU.Core;

public class OperatorINCReg16 : CPUOperator
{
	public OperatorINCReg16() : base("INC", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		var value = cpu.Get16BitRegister(register);
		cpu.Set16BitRegister(register, (ushort)(value + 1));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode)
	{
		return opcode switch
		{
			0x03 => CPURegister.BC,
			0x13 => CPURegister.DE,
			0x23 => CPURegister.HL,
			0x33 => CPURegister.SP,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		return base.ToString() + $" ${register}";
	}
}