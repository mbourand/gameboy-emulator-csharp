namespace GBMU.Core;

public class OperatorLDReg8AReg16 : CPUOperator
{
	public OperatorLDReg8AReg16() : base("LD", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var srcRegister = GetSourceRegister(opcode);
		var dstRegister = GetDestinationRegister(opcode);

		var srcAddr = cpu.Get16BitRegister(srcRegister);

		cpu.Set8BitRegister(dstRegister, memory.ReadByte(srcAddr));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetDestinationRegister(int opcode)
	{
		if (opcode == 0x0A || opcode == 0x1A)
			return CPURegister.A;

		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.HL /* Unused */, CPURegister.A };
		var index = ByteUtils.LowNibble((byte)opcode) / 2;
		return registers[index];
	}

	private CPURegister GetSourceRegister(int opcode)
	{
		return opcode switch
		{
			0x0A => CPURegister.BC,
			0x1A => CPURegister.DE,
			_ => CPURegister.HL
		};
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var sourceRegister = GetSourceRegister(opcode).ToString();
		var destinationRegister = GetDestinationRegister(opcode).ToString();
		return base.ToString() + $" ${destinationRegister}, (${sourceRegister})";
	}
}