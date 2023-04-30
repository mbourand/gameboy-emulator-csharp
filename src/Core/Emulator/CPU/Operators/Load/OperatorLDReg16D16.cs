namespace GBMU.Core;

public class OperatorLDReg16D16 : CPUOperator
{
	public OperatorLDReg16D16() : base("LD", 3) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		cpu.Set16BitRegister(register, memory.ReadWord((ushort)(cpu.PC + 1)));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode)
	{
		return opcode switch
		{
			0x01 => CPURegister.BC,
			0x11 => CPURegister.DE,
			0x21 => CPURegister.HL,
			0x31 => CPURegister.SP,
			_ => throw new System.Exception("Invalid opcode")
		};
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		var value = memory.ReadWord((ushort)(addr + 1));
		return base.ToString() + $" ${register}, ${value:X4}";
	}
}