using System;
using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 8 bits
public class DataTypeU8Address : OperationDataType {
	private ushort? _lastRegisteredValue;

	public DataTypeU8Address() {
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		var address = memory.ReadByte((ushort)(cpu.PC + 1));
		_lastRegisteredValue = memory.ReadByte((ushort)(0xFF00 + address));
		return memory.ReadByte((ushort)(0xFF00 + address));
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) {
		var address = memory.ReadByte((ushort)(cpu.PC + 1));
		_lastRegisteredValue = address;
		memory.WriteByte((ushort)(0xFF00 + address), (byte)(value & 0xFF));
	}

	public byte GetLength() => 1;

	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"(0xFF00 + U8) {{0x{_lastRegisteredValue.Value:X2}}}";
		}
		return "(0xFF00 + U8)";
	}
}