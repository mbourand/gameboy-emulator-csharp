namespace GBMU.Core;

public class OperatorDECReg16 : CPUOperator
{
	public OperatorDECReg16() : base("DEC", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		var value = cpu.Get16BitRegister(register);
		cpu.Set16BitRegister(register, (ushort)(value - 1));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode)
	{
		return opcode switch
		{
			0x0B => CPURegister.BC,
			0x1B => CPURegister.DE,
			0x2B => CPURegister.HL,
			0x3B => CPURegister.SP,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		return base.ToString() + $" ${register}";
	}
}