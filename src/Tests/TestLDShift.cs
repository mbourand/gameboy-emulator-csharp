using System;
using System.IO;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestLDShift : Test {
	public static void TestLDShiftReg8Reg16Address(CPURegister source, CPURegister destination, ushort destinationAddress, byte sourceValue, ShiftingBehaviour behaviour) {
		var gameboy = TestUtils.InitGameboy();

		var sourceDataType = new DataTypeReg8(source);
		sourceDataType.WriteToDestination(gameboy.CPU, gameboy.Memory, sourceValue);

		var destinationDataType = new DataTypeReg16Address(destination);
		gameboy.CPU.Set16BitRegister(destination, destinationAddress);

		var operation = new OperatorLDShift(sourceDataType, destinationDataType, destination, behaviour);
		operation.Execute(gameboy.CPU, gameboy.Memory, 0x00);

		var offset = behaviour == ShiftingBehaviour.Increment ? 1 : -1;
		var valueWasSet = sourceValue == gameboy.Memory.ReadByte(destinationAddress);
		var offsetWasApplied = gameboy.CPU.Get16BitRegister(destination) == destinationAddress + offset;
		Expect(gameboy, operation, valueWasSet && offsetWasApplied);
	}

	public static void TestLDShiftReg16AddressReg8(CPURegister source, CPURegister destination, ushort sourceAddress, byte sourceValue, ShiftingBehaviour behaviour) {
		var gameboy = TestUtils.InitGameboy();

		var sourceDataType = new DataTypeReg16Address(source);
		gameboy.CPU.Set16BitRegister(source, sourceAddress);
		sourceDataType.WriteToDestination(gameboy.CPU, gameboy.Memory, sourceValue);

		var destinationDataType = new DataTypeReg8(destination);

		var operation = new OperatorLDShift(sourceDataType, destinationDataType, source, behaviour);
		operation.Execute(gameboy.CPU, gameboy.Memory, 0x00);

		var offset = behaviour == ShiftingBehaviour.Increment ? 1 : -1;
		var valueWasSet = sourceValue == destinationDataType.GetSourceValue(gameboy.CPU, gameboy.Memory);
		var offsetWasApplied = gameboy.CPU.Get16BitRegister(source) == sourceAddress + offset;
		Expect(gameboy, operation, valueWasSet && offsetWasApplied);
	}

	public override void Run() {
		TestLDShiftReg8Reg16Address(CPURegister.A, CPURegister.HL, Memory.WorkRAMBank0.Address, 0x42, ShiftingBehaviour.Increment);
		TestLDShiftReg8Reg16Address(CPURegister.A, CPURegister.HL, Memory.WorkRAMBank0.Address, 0x42, ShiftingBehaviour.Decrement);

		TestLDShiftReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.Address, 0x42, ShiftingBehaviour.Increment);
		TestLDShiftReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.Address, 0x42, ShiftingBehaviour.Decrement);

		Console.WriteLine("All LD Shift tests passed");
	}

	public static void Expect(Gameboy gameboy, CPUOperator operation, bool condition) {
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + " failed");
		Console.WriteLine(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + " succeeded");
	}
}