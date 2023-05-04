using System;
using System.IO;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestLDShift : Test
{
	public void TestLDShiftReg8Reg16Address(CPURegister source, CPURegister destination, ushort destinationAddress, byte sourceValue, ShiftingBehaviour behaviour)
	{
		var gameboy = TestUtils.InitGameboy();

		var sourceDataType = new DataTypeReg8(source);
		sourceDataType.WriteToDestination(gameboy.cpu, gameboy.memory, sourceValue);

		var destinationDataType = new DataTypeReg16Address(destination);
		gameboy.cpu.Set16BitRegister(destination, destinationAddress);

		var operation = new OperatorLDShift(sourceDataType, destinationDataType, destination, behaviour);
		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		var offset = behaviour == ShiftingBehaviour.INCREMENT ? 1 : -1;
		var valueWasSet = sourceValue == gameboy.memory.ReadByte(destinationAddress);
		var offsetWasApplied = gameboy.cpu.Get16BitRegister(destination) == destinationAddress + offset;
		Expect(gameboy, operation, valueWasSet && offsetWasApplied);
	}

	public void TestLDShiftReg16AddressReg8(CPURegister source, CPURegister destination, ushort sourceAddress, byte sourceValue, ShiftingBehaviour behaviour)
	{
		var gameboy = TestUtils.InitGameboy();

		var sourceDataType = new DataTypeReg16Address(source);
		gameboy.cpu.Set16BitRegister(source, sourceAddress);
		sourceDataType.WriteToDestination(gameboy.cpu, gameboy.memory, sourceValue);

		var destinationDataType = new DataTypeReg8(destination);

		var operation = new OperatorLDShift(sourceDataType, destinationDataType, source, behaviour);
		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		var offset = behaviour == ShiftingBehaviour.INCREMENT ? 1 : -1;
		var valueWasSet = sourceValue == destinationDataType.GetSourceValue(gameboy.cpu, gameboy.memory);
		var offsetWasApplied = gameboy.cpu.Get16BitRegister(source) == sourceAddress + offset;
		Expect(gameboy, operation, valueWasSet && offsetWasApplied);
	}

	public override void Run()
	{
		TestLDShiftReg8Reg16Address(CPURegister.A, CPURegister.HL, Memory.WorkRAMBank0.addr, 0x42, ShiftingBehaviour.INCREMENT);
		TestLDShiftReg8Reg16Address(CPURegister.A, CPURegister.HL, Memory.WorkRAMBank0.addr, 0x42, ShiftingBehaviour.DECREMENT);

		TestLDShiftReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.addr, 0x42, ShiftingBehaviour.INCREMENT);
		TestLDShiftReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.addr, 0x42, ShiftingBehaviour.DECREMENT);

		Console.WriteLine("All LD Shift tests passed");
	}

	public void Expect(Gameboy gameboy, CPUOperator operation, bool condition)
	{
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " failed");
		Console.WriteLine(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " succeeded");
	}
}