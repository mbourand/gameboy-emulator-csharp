namespace GBMU.Core;

public class OperatorRETFlag : CPUOperator {
	private readonly CPUFlag _flag;
	private readonly bool _expectedValue;

	private bool _branched;

	public OperatorRETFlag(CPUFlag flag, bool expectedValue) : base("RET", 1) {
		_flag = flag;
		_expectedValue = expectedValue;
		_branched = false;
	}

	public override void Execute(CPU cpu, Memory memory, int opcode) {
		_branched = cpu.GetFlag(_flag) == _expectedValue;
		if (_branched)
			cpu.PC = cpu.PopFromStack();

		base.Execute(cpu, memory, opcode);
	}

	public override void ShiftPC(CPU cpu, Memory memory) {
		if (!_branched)
			base.ShiftPC(cpu, memory);
	}

	public override int GetCycles(bool cbPrefix, byte opcode) => _branched ? 20 : 8;

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr) {
		return base.ToString(cpu, memory, opcode, addr) + $" {FlagUtils.FlagConditionToMnemonic(_flag, _expectedValue)}";
	}
}