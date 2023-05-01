using System;
using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 8 bits
public class DataTypeU8Address : OperationDataType
{
	public ushort GetSourceValue(CPU cpu, Memory memory)
	{
		var value = memory.ReadByte((ushort)(cpu.PC + 1));
		return memory.ReadByte((ushort)(0xFF00 + value));
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value)
	{
		var address = memory.ReadByte((ushort)(cpu.PC + 1));
		memory.WriteByte((ushort)(0xFF00 + address), (byte)(value & 0xFF));
	}

	public byte GetLength() => 1;
	public string GetMnemonic() => "(U8)";
}