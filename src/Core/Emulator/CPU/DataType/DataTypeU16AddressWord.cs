using GBMU.Core;

namespace GBMU;

// Only used for the LD (U16), SP instruction
public class DataTypeU16AddressWord : OperationDataType
{
	public ushort GetSourceValue(CPU cpu, Memory memory)
	{
		var value = memory.ReadWord((ushort)(cpu.PC + 1));
		return memory.ReadWord(value);
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value)
	{
		var address = memory.ReadWord((ushort)(cpu.PC + 1));
		memory.WriteWord(address, value);
	}

	public byte GetLength() => 2;
	public string GetMnemonic() => "(U16)";
}