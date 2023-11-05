using System;
using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 8 bits
public class DataTypeU8 : OperationDataType {
	private ushort? _lastRegisteredValue;

	public DataTypeU8() {
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		_lastRegisteredValue = memory.ReadByte((ushort)(cpu.PC + 1));
		return memory.ReadByte((ushort)(cpu.PC + 1));
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => throw new Exception("Cannot write to U8 if not address");

	public byte GetLength() => 1;
	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"U8 {{0x{_lastRegisteredValue.Value:X2}}}";
		}
		return "U8";
	}
}