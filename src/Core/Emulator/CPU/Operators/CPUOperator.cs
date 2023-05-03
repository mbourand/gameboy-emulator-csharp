namespace GBMU.Core;

public abstract class CPUOperator
{
	public string name;
	public byte length;

	public CPUOperator(string name, byte length)
	{
		this.name = name;
		this.length = length;
	}

	public virtual void Execute(CPU cpu, Memory memory, int opcode)
	{
	}

	public virtual string ToString(CPU cpu, Memory memory, int opcode, ushort addr) => name.PadRight(4);
}