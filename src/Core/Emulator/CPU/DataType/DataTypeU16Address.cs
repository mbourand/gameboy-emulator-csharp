using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 16 bits
public class DataTypeU16Address : OperationDataType {
	private ushort? _lastRegisteredValue;

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		var address = memory.ReadWord((ushort)(cpu.PC + 1));
		_lastRegisteredValue = memory.ReadByte(address);
		return memory.ReadByte(address);
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) {
		var address = memory.ReadWord((ushort)(cpu.PC + 1));
		_lastRegisteredValue = address;
		memory.WriteByte(address, (byte)(value & 0xFF));
	}

	public byte GetLength() => 2;

	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"(U16) {{0x{_lastRegisteredValue.Value:X2}}}";
		}
		return "(U16)";
	}
}