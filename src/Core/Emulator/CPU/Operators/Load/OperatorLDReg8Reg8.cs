namespace GBMU.Core;

public class OperatorLDReg8Reg8 : CPUOperator
{
	public OperatorLDReg8Reg8() : base("LD", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceRegister = GetSourceRegister(opcode);
		var destinationRegister = GetDestinationRegister(opcode);
		cpu.Set8BitRegister(destinationRegister, cpu.Get8BitRegister(sourceRegister));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetSourceRegister(int opcode)
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.HL /* Unused */, CPURegister.A };
		var index = ByteUtils.LowNibble((byte)opcode) / 2;
		return registers[index];
	}

	private CPURegister GetDestinationRegister(int opcode)
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.HL /* Unused */, CPURegister.A };
		var index = (byte)((opcode - 0x40) / 8);
		return registers[index];
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var sourceRegister = GetSourceRegister(opcode).ToString();
		var destinationRegister = GetDestinationRegister(opcode).ToString();
		return base.ToString() + $" ${destinationRegister}, ${sourceRegister}";
	}
}