namespace GBMU.Core;

public class OperatorRLCA : CPUOperator
{
	public OperatorRLCA() : base("RLCA", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		bool willOverflow = (cpu.A & 0b1000_0000) != 0;
		cpu.ResetFlags();
		cpu.SetFlag(CPUFlag.CARRY, willOverflow);

		cpu.A = (byte)((cpu.A << 1) | (cpu.A >> 7));

		base.Execute(cpu, memory, opcode);
	}
}