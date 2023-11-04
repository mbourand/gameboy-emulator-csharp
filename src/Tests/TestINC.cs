using System;
using System.Collections.Generic;

using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestINC : Test {
	public static bool CheckFlags(Dictionary<CPUFlag, bool> expectedValues, CPU cpu) {
		foreach (var flag in expectedValues.Keys)
			if (expectedValues[flag] != cpu.GetFlag(flag))
				return false;
		return true;
	}

	public static void RunOperation(Gameboy gameboy, OperationDataType dataType, ushort inputValue, ushort expectedOutput, Dictionary<CPUFlag, bool> expectedFlags, FlagPermissionHandler flagPermissionHandler) {
		dataType.WriteToDestination(gameboy.CPU, gameboy.Memory, inputValue);

		var operation = new OperatorINC(dataType, flagPermissionHandler);
		operation.Execute(gameboy.CPU, gameboy.Memory, 0x00);

		var valueWasSet = expectedOutput == dataType.GetSourceValue(gameboy.CPU, gameboy.Memory);
		var flagsWereSet = CheckFlags(expectedFlags, gameboy.CPU);
		Expect(gameboy, operation, valueWasSet && flagsWereSet, inputValue);
	}

	public static void TestINC8Bit(OperationDataType dataType) {
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[] {
			(0x00, 0x01, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x0F, 0x10, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
			(0x42, 0x43, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0xFF, 0x00, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
		};

		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy, dataType, test.Item1, test.Item2, test.Item3, new FlagPermissionHandler("Z0H-"));
		}
	}

	public static void TestINC16Bit(OperationDataType dataType) {
		var tests = new (ushort, ushort, Dictionary<CPUFlag, bool>)[]
		{
			(0x0000, 0x0001, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x000F, 0x0010, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x0042, 0x0043, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x00FF, 0x0100, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x0100, 0x0101, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x0FFF, 0x1000, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x4269, 0x426A, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0xFFFF, 0x0000, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		};

		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy, dataType, test.Item1, test.Item2, test.Item3, new FlagPermissionHandler("----"));
		}
	}

	public static void TestINCReg16Address(CPURegister register, ushort registerAddress) {
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0x00, 0x01, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0x0F, 0x10, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
			(0x42, 0x43, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
			(0xFF, 0x00, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, false}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
		};

		var dataType = new DataTypeReg16Address(register);

		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			gameboy.CPU.Set16BitRegister(register, registerAddress);
			RunOperation(gameboy, dataType, test.Item1, test.Item2, test.Item3, new FlagPermissionHandler("Z0H-"));
		}
	}


	public override void Run() {
		var registers8Bits = new CPURegister[] { CPURegister.A, CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L };

		foreach (var register in registers8Bits)
			TestINC8Bit(new DataTypeReg8(register));

		var registers16Bits = new CPURegister[] { CPURegister.BC, CPURegister.DE, CPURegister.HL, CPURegister.SP };
		foreach (var register in registers16Bits)
			TestINC16Bit(new DataTypeReg16(register));

		TestINCReg16Address(CPURegister.HL, Memory.WorkRAMBank0.Address);

		Console.WriteLine("All INC tests passed");
	}

	public static void Expect(Gameboy gameboy, CPUOperator operation, bool condition, ushort inputValue) {
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + $" failed with input value 0x{inputValue:X2}");
		Console.WriteLine(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + $" succeeded with input value 0x{inputValue:X2}");
	}
}