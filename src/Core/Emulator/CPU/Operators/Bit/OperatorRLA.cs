namespace GBMU.Core;

public class OperatorRLA : CPUOperator
{
	public OperatorRLA() : base("RLA", 1) { }

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		bool wasCarrySet = cpu.GetFlag(CPUFlag.CARRY);
		bool willOverflow = (cpu.A & 0b1000_0000) != 0;
		cpu.ResetFlags();
		cpu.SetFlag(CPUFlag.CARRY, willOverflow);
		cpu.A = (byte)((cpu.A << 1) | (wasCarrySet ? 1 : 0));

		base.Execute(cpu, memory, opcode);
	}
}