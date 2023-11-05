using GBMU.Core;

namespace GBMU;

// Cette classe contient une logique générale permettant de charger et assigner des données via un entier non signé 16 bits
public class DataTypeU16 : OperationDataType {
	private ushort? _lastRegisteredValue;

	public DataTypeU16() {
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		_lastRegisteredValue = memory.ReadWord((ushort)(cpu.PC + 1));
		return memory.ReadWord((ushort)(cpu.PC + 1));
	}
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => throw new System.Exception("Cannot write to U16 if not address");

	public byte GetLength() => 2;
	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"U16 {{0x{_lastRegisteredValue.Value:X4}}}";
		}
		return "U16";
	}
}