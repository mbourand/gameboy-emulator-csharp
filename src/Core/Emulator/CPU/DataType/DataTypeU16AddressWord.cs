using GBMU.Core;

namespace GBMU;

// Only used for the LD (U16), SP instruction
public class DataTypeU16AddressWord : OperationDataType {
	private ushort? _lastRegisteredValue;

	public DataTypeU16AddressWord() {
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		var address = memory.ReadWord((ushort)(cpu.PC + 1));
		_lastRegisteredValue = memory.ReadWord(address);
		return memory.ReadWord(address);
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) {
		var address = memory.ReadWord((ushort)(cpu.PC + 1));
		_lastRegisteredValue = address;
		memory.WriteWord(address, value);
	}

	public byte GetLength() => 2;
	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"(U16) {{0x{_lastRegisteredValue.Value:X4}}}";
		}
		return "(U16)";
	}
}