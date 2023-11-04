namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 16 bits
public class DataTypeReg16Address : OperationDataType {
	private readonly CPURegister _register;

	public DataTypeReg16Address(CPURegister register) => _register = register;

	public ushort GetSourceValue(CPU cpu, Memory memory) => memory.ReadByte(cpu.Get16BitRegister(_register));
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => memory.WriteByte(cpu.Get16BitRegister(_register), (byte)(value & 0xFF));

	public byte GetLength() => 0;
	public string GetMnemonic() => $"({_register.ToString()})";
}