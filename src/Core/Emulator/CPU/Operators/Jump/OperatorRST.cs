namespace GBMU.Core;

public class OperatorRST : CPUOperator
{
	private readonly ushort _address;

	public OperatorRST(ushort address) : base("RST", 1)
	{
		_address = address;
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		cpu.PushToStack((ushort)(cpu.PC + length));
		cpu.PC = _address;
		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) { }

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => base.ToString(cpu, memory, opcode, addr);
}