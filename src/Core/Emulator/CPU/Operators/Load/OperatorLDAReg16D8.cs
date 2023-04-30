namespace GBMU.Core;

public class OperatorLDAReg16D8 : CPUOperator
{
	public OperatorLDAReg16D8() : base("LD", 2) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		var addr = cpu.Get16BitRegister(register);
		memory.WriteByte(addr, memory.ReadByte((ushort)(cpu.PC + 1)));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode) => CPURegister.HL;

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		var value = memory.ReadByte((ushort)(addr + 1));
		return base.ToString() + $" ${register}, ${value:X2}";
	}
}