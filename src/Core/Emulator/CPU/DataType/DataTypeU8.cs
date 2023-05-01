using System;
using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 8 bits
public class DataTypeU8 : OperationDataType
{
	public ushort GetSourceValue(CPU cpu, Memory memory) => memory.ReadByte((ushort)(cpu.PC + 1));
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => throw new Exception("Cannot write to U8 if not address");

	public byte GetLength() => 1;
	public string GetMnemonic() => "U8";
}