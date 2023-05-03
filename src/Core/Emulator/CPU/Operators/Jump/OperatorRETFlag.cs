namespace GBMU.Core;

public class OperatorRETFlag : CPUOperator
{
	private CPUFlag _flag;
	private bool _expectedValue;

	public OperatorRETFlag(CPUFlag flag, bool expectedValue) : base("RET", 1)
	{
		_flag = flag;
		_expectedValue = expectedValue;
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		if (cpu.GetFlag(_flag) == _expectedValue)
			cpu.PC = cpu.PopFromStack();

		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString() + $" ${FlagUtils.FlagConditionToMnemonic(_flag, _expectedValue)}";
	}
}