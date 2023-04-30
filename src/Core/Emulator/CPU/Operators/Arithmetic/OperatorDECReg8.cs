namespace GBMU.Core;

public class OperatorDECReg8 : CPUOperator
{
	public OperatorDECReg8() : base("DEC", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		var value = cpu.Get8BitRegister(register);

		cpu.SetFlag(CPUFlag.N_SUBTRACT, true);
		cpu.SetFlag(CPUFlag.HALF_CARRY, (value & 0x0F) == 0x00);
		cpu.SetFlag(CPUFlag.ZERO, value == 0x01);

		cpu.Set8BitRegister(register, (byte)(value - 1));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode)
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.HL /* Unused */, CPURegister.A };
		var index = ByteUtils.HighNibble((byte)opcode) * 2 + ByteUtils.LowNibble((byte)opcode) / 8;
		return registers[index];
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		return base.ToString() + $" ${register}";
	}
}