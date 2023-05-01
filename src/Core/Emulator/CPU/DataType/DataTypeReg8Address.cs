namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 8 bits
public class DataTypeReg8Address : OperationDataType
{
	private CPURegister _register;

	public DataTypeReg8Address(CPURegister register) => _register = register;

	public ushort GetSourceValue(CPU cpu, Memory memory) => memory.ReadByte((ushort)(0xFF00 + cpu.Get8BitRegister(_register)));
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => memory.WriteByte((ushort)(0xFF00 + cpu.Get8BitRegister(_register)), (byte)(value & 0xFF));

	public byte GetLength() => 0;
	public string GetMnemonic() => $"({_register.ToString()})";
}