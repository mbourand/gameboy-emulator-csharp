using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 16 bits
public class DataTypeU16Address : OperationDataType
{
	public ushort GetSourceValue(CPU cpu, Memory memory)
	{
		var value = memory.ReadWord((ushort)(cpu.PC + 1));
		return memory.ReadByte(value);
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value)
	{
		var address = memory.ReadWord((ushort)(cpu.PC + 1));
		memory.WriteByte(address, (byte)(value & 0xFF));
	}

	public byte GetLength() => 2;
	public string GetMnemonic() => "(U16)";
}