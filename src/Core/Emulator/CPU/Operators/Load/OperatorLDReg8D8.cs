namespace GBMU.Core;

public class OperatorLDReg8D8 : CPUOperator
{
	public OperatorLDReg8D8() : base("LD", 2) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		cpu.Set16BitRegister(register, memory.ReadByte((ushort)(cpu.PC + 1)));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode)
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.HL /* Unused */, CPURegister.A };
		var index = ByteUtils.LowNibble((byte)opcode) / 8;
		return registers[index];
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		var value = memory.ReadByte((ushort)(addr + 1));
		return base.ToString() + $" ${register}, ${value:X2}";
	}
}