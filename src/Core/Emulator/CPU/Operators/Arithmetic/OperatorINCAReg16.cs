namespace GBMU.Core;

public class OperatorINCAReg16 : CPUOperator
{
	public OperatorINCAReg16() : base("INC", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var register = GetRegister(opcode);
		var addr = cpu.Get16BitRegister(register);
		memory.WriteByte(addr, (byte)(memory.ReadByte(addr) + 1));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode) => CPURegister.HL;

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		return base.ToString() + $" (${register})";
	}
}