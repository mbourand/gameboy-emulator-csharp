namespace GBMU.Core;

public class OperatorLDA16Reg16 : CPUOperator
{
	public OperatorLDA16Reg16() : base("LD", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var addr = memory.ReadWord((ushort)(cpu.PC + 1));
		var register = GetRegister(opcode);
		memory.WriteWord(addr, cpu.Get16BitRegister(register));
		base.Execute(cpu, memory, opcode);
	}

	private CPURegister GetRegister(int opcode) => CPURegister.SP;

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		var register = GetRegister(opcode).ToString();
		var dstAddr = memory.ReadWord((ushort)(addr + 1));
		return base.ToString() + $" (${dstAddr:X4}), ${register}";
	}
}