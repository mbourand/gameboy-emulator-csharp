namespace GBMU.Core;

public class OperatorRRCA : CPUOperator
{
	public OperatorRRCA() : base("RRCA", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		bool willOverflow = (cpu.A & 0b0000_0001) != 0;
		cpu.ResetFlags();
		cpu.SetFlag(CPUFlag.CARRY, willOverflow);

		cpu.A = (byte)((cpu.A >> 1) | (cpu.A << 7));

		base.Execute(cpu, memory, opcode);
	}
}