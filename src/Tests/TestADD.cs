using System;
using System.Collections.Generic;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestADD : Test {
	public static bool CheckFlags(Dictionary<CPUFlag, bool> expectedValues, CPU cpu) {
		foreach (var flag in expectedValues.Keys)
			if (expectedValues[flag] != cpu.GetFlag(flag))
				return false;
		return true;
	}

	public static void RunOperation(
		Gameboy gameboy,
		OperationDataType source,
		OperationDataType destination,
		ushort sourceValue,
		ushort destinationValue,
		ushort expectedOutput,
		Dictionary<CPUFlag, bool> expectedFlags,
		FlagPermissionHandler flagPermissionHandler,
		CarryBit carryBit = CarryBit.Bit8,
		HalfCarryBit halfCarryBit = HalfCarryBit.Bit4,
		bool isSourceSigned = false
	) {
		try {
			source.WriteToDestination(gameboy.CPU, gameboy.Memory, sourceValue);
		} catch (Exception) { }
		destination.WriteToDestination(gameboy.CPU, gameboy.Memory, destinationValue);

		var operation = new OperatorADD(source, destination, flagPermissionHandler, carryBit, halfCarryBit, isSourceSigned);
		operation.Execute(gameboy.CPU, gameboy.Memory, 0x00);

		var valueWasSet = expectedOutput == destination.GetSourceValue(gameboy.CPU, gameboy.Memory);
		var flagsWereSet = CheckFlags(expectedFlags, gameboy.CPU);
		Expect(gameboy, operation, valueWasSet && flagsWereSet, sourceValue, destinationValue);
	}

	public static void TestADDReg8Reg8(CPURegister source, CPURegister destination) {
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0xFF, 0x00, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x01, 0x10, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x42, 0x69, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0xE9, 0x07, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
			(0xFF, 0xFF, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
			(0x00, 0x00, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		};

		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy,
				new DataTypeReg8(source),
				new DataTypeReg8(destination), test.Item1, test.Item2, (byte)(test.Item1 + test.Item2), test.Item3, new FlagPermissionHandler("Z0HC"));
		}
	}

	public static void TestADDU8Reg8(CPURegister destination) {
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0xFF, 0x00, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x01, 0x10, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x42, 0x69, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0xE9, 0x07, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
			(0xFF, 0xFF, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
			(0x00, 0x00, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		};

		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			gameboy.Memory.WriteByte((ushort)(gameboy.CPU.PC + 1), test.Item1);
			RunOperation(gameboy,
				new DataTypeU8(),
				new DataTypeReg8(destination),
				test.Item1,
				test.Item2,
				(byte)(test.Item1 + test.Item2),
				test.Item3,
				new FlagPermissionHandler("Z0HC")
			);
		}
	}

	public static void TestADDReg16AddressReg8(CPURegister source, CPURegister destination, ushort sourceAddress) {
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0xFF, 0x00, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x01, 0x10, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x42, 0x69, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0xE9, 0x07, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
			(0xFF, 0xFF, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
			(0x00, 0x00, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		};

		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			gameboy.CPU.Set16BitRegister(source, sourceAddress);
			RunOperation(gameboy, new DataTypeReg16Address(source), new DataTypeReg8(destination), test.Item1, test.Item2, (byte)(test.Item1 + test.Item2), test.Item3, new FlagPermissionHandler("Z0HC"));
		}
	}

	public override void Run() {
		TestADDReg8Reg8(CPURegister.B, CPURegister.A);
		TestADDReg8Reg8(CPURegister.C, CPURegister.A);
		TestADDReg8Reg8(CPURegister.D, CPURegister.A);
		TestADDReg8Reg8(CPURegister.E, CPURegister.A);
		TestADDReg8Reg8(CPURegister.H, CPURegister.A);
		TestADDReg8Reg8(CPURegister.L, CPURegister.A);

		TestADDReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.Address);

		TestADDU8Reg8(CPURegister.A);

		Console.WriteLine("All ADD tests passed");
	}

	public static void Expect(Gameboy gameboy, CPUOperator operation, bool condition, ushort sourceValue, ushort destinationValue) {
		if (!condition)
			throw new Exception(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + $" failed for {destinationValue} + {sourceValue}");
		Console.WriteLine(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + $" succeeded for {destinationValue} + {sourceValue}");
	}
}