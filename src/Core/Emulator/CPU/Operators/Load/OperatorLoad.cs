using System;

namespace GBMU.Core;

public enum LoadDataType
{
	VALUE,
	REGISTER,
}

public enum LoadDataSize
{
	BYTE,
	WORD,
}

public enum LoadDataTreatment
{
	DEFAULT,
	ADDRESS
}

public class OperatorLoad : CPUOperator
{
	private LoadDataType _sourceDataType;
	private LoadDataSize _sourceDataSize;
	private LoadDataTreatment _sourceDataTreatment;
	private LoadDataSize _destinationDataSize;
	private LoadDataTreatment _destinationDataTreatment;
	private CPURegister _sourceRegister;
	private CPURegister _destinationRegister;

	public OperatorLoad(
		byte length,
		LoadDataType sourceDataType,
		LoadDataSize sourceDataSize,
		LoadDataTreatment sourceDataTreatment,
		LoadDataSize destinationDataSize,
		LoadDataTreatment destinationDataTreatment,
		CPURegister source = CPURegister.NONE,
		CPURegister dest = CPURegister.NONE) : base("LD", length)
	{
		_sourceDataType = sourceDataType;
		_sourceDataSize = sourceDataSize;
		_sourceDataTreatment = sourceDataTreatment;
		_destinationDataSize = destinationDataSize;
		_destinationDataTreatment = destinationDataTreatment;
		_sourceRegister = source;
		_destinationRegister = dest;

		if (_sourceDataType == LoadDataType.REGISTER && _sourceRegister == CPURegister.NONE)
			throw new Exception("Invalid combination: register source type and none register");
		if (_destinationDataTreatment == LoadDataTreatment.DEFAULT && _destinationRegister == CPURegister.NONE)
			throw new Exception("Invalid combination: register destination type and none register");
		if (_destinationDataTreatment == LoadDataTreatment.ADDRESS && _destinationDataSize == LoadDataSize.BYTE)
			throw new Exception("Invalid combination: address destination type and byte size");
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var sourceValue = GetSourceValue(cpu, memory, opcode);
		WriteToDestination(cpu, memory, opcode, sourceValue);
		base.Execute(cpu, memory, opcode);
	}

	private ushort GetSourceValue(CPU cpu, Memory memory, int opcode)
	{
		ushort valueFromType = 0;
		if (_sourceDataType == LoadDataType.VALUE && _sourceDataSize == LoadDataSize.BYTE)
			valueFromType = memory.ReadByte((ushort)(cpu.PC + 1));
		else if (_sourceDataType == LoadDataType.VALUE && _sourceDataSize == LoadDataSize.WORD)
			valueFromType = memory.ReadWord((ushort)(cpu.PC + 1));
		else if (_sourceDataType == LoadDataType.REGISTER && _sourceDataSize == LoadDataSize.BYTE)
			valueFromType = cpu.Get8BitRegister(_sourceRegister);
		else if (_sourceDataType == LoadDataType.REGISTER && _sourceDataSize == LoadDataSize.WORD)
			valueFromType = cpu.Get16BitRegister(_sourceRegister);

		var treatedValue = _sourceDataTreatment switch
		{
			LoadDataTreatment.DEFAULT => valueFromType,
			LoadDataTreatment.ADDRESS => memory.ReadWord(valueFromType),
			_ => throw new Exception("Invalid source data treatment")
		};

		return treatedValue;
	}

	private void WriteToDestination(CPU cpu, Memory memory, int opcode, ushort value)
	{
		switch (_destinationDataTreatment)
		{
			case LoadDataTreatment.DEFAULT:
				WriteToRegister(cpu, memory, opcode, value);
				break;
			case LoadDataTreatment.ADDRESS:
				WriteToAddress(cpu, memory, opcode, value);
				break;
		}
	}

	private void WriteToRegister(CPU cpu, Memory memory, int opcode, ushort value)
	{
		switch (_destinationDataSize)
		{
			case LoadDataSize.BYTE:
				cpu.Set8BitRegister(_destinationRegister, (byte)value);
				break;
			case LoadDataSize.WORD:
				cpu.Set16BitRegister(_destinationRegister, value);
				break;
		}
	}

	private void WriteToAddress(CPU cpu, Memory memory, int opcode, ushort value)
	{
		var addr = memory.ReadWord((ushort)(cpu.PC + 1));
		memory.WriteWord(addr, value);
	}
}