namespace GBMU.Core;

public class OperatorLDAReg16Reg8 : CPUOperator
{
	public OperatorLDAReg16Reg8() : base("LD", 2) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var srcRegister = GetSourceRegister(opcode);
		var dstRegister = GetDestinationRegister(opcode);

		var srcValue = cpu.Get8BitRegister(srcRegister);
		var dstAddr = cpu.Get16BitRegister(dstRegister);

		memory.WriteByte(dstAddr, srcValue);
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetDestinationRegister(int opcode)
	{
		if (opcode == 0x02)
			return CPURegister.BC;
		else if (opcode == 0x12)
			return CPURegister.DE;
		else if (ByteUtils.HighNibble((byte)opcode) == 0x7)
			return CPURegister.HL;
		throw new System.Exception("Invalid opcode");
	}

	private CPURegister GetSourceRegister(int opcode)
	{
		if (opcode == 0x22 || opcode == 0x32)
			return CPURegister.A;

		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.HL /* Unused */, CPURegister.A };
		var index = ByteUtils.LowNibble((byte)opcode) / 2;
		return registers[index];
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var sourceRegister = GetSourceRegister(opcode).ToString();
		var destinationRegister = GetDestinationRegister(opcode).ToString();
		return base.ToString() + $" (${destinationRegister}), ${sourceRegister}";
	}
}