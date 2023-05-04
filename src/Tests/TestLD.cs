using System;
using System.IO;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestLD : Test
{
	public void BaseTestLD(Gameboy gameboy, OperationDataType sourceDataType, OperationDataType destinationType, ushort sourceValue)
	{
		try
		{
			sourceDataType.WriteToDestination(gameboy.cpu, gameboy.memory, sourceValue);
		}
		catch (Exception) { }

		var operation = new OperatorLD(sourceDataType, destinationType);
		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		Expect(gameboy, operation, sourceValue == destinationType.GetSourceValue(gameboy.cpu, gameboy.memory));
	}

	public void TestLDReg8Reg8(CPURegister source, CPURegister destination, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		BaseTestLD(gameboy, new DataTypeReg8(source), new DataTypeReg8(destination), sourceValue);
	}

	public void TestLDReg16AddressReg8(CPURegister sourceAddressRegister, CPURegister destinationRegister, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		// Put the source register to a workable address
		gameboy.cpu.Set16BitRegister(sourceAddressRegister, Memory.WorkRAMBank0.addr);
		BaseTestLD(gameboy, new DataTypeReg16Address(sourceAddressRegister), new DataTypeReg8(destinationRegister), sourceValue);
	}

	public void TestLDReg8Reg16Address(CPURegister sourceRegister, CPURegister destinationAddressRegister, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		// Put the destination register to a workable address
		gameboy.cpu.Set16BitRegister(destinationAddressRegister, Memory.WorkRAMBank0.addr);
		BaseTestLD(gameboy, new DataTypeReg8(sourceRegister), new DataTypeReg16Address(destinationAddressRegister), sourceValue);
	}

	public void TestLDU8Reg8(CPURegister destinationRegister, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		// Emulate the U8 value in the ROM
		gameboy.cartridge.WriteByte((ushort)(gameboy.cpu.PC + 1), sourceValue);
		BaseTestLD(gameboy, new DataTypeU8(), new DataTypeReg8(destinationRegister), sourceValue);
	}

	public void TestLDU8Reg16Address(CPURegister destinationRegister, ushort destinationAddress, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		gameboy.cpu.Set16BitRegister(destinationRegister, destinationAddress);
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), sourceValue);
		BaseTestLD(gameboy, new DataTypeU8(), new DataTypeReg16Address(destinationRegister), sourceValue);
	}

	public void TestLDReg8U16Address(CPURegister sourceRegister, ushort destinationAddress, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		// Emulate the U16 address in the ROM
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), destinationAddress);
		BaseTestLD(gameboy, new DataTypeReg8(sourceRegister), new DataTypeU16Address(), sourceValue);
	}

	public void TestLDU16AddressReg8(CPURegister sourceRegister, ushort sourceAddress, byte sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		// Emulate the U16 address in the ROM
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), sourceAddress);
		BaseTestLD(gameboy, new DataTypeU16Address(), new DataTypeReg8(sourceRegister), sourceValue);
	}

	public void TestLDReg8Reg8Address(CPURegister sourceRegister, CPURegister destinationRegister, byte sourceValue, byte destinationAddress)
	{
		var gameboy = TestUtils.InitGameboy();
		gameboy.cpu.Set8BitRegister(destinationRegister, destinationAddress);
		BaseTestLD(gameboy, new DataTypeReg8(sourceRegister), new DataTypeReg8Address(destinationRegister), sourceValue);
	}

	public void TestLDReg8AddressReg8(CPURegister sourceRegister, CPURegister destinationRegister, byte sourceValue, byte sourceAddress)
	{
		var gameboy = TestUtils.InitGameboy();
		gameboy.cpu.Set8BitRegister(sourceRegister, sourceAddress);
		BaseTestLD(gameboy, new DataTypeReg8Address(sourceRegister), new DataTypeReg8(destinationRegister), sourceValue);
	}

	public void TestLDU8AddressReg8(CPURegister destinationRegister, byte sourceValue, byte sourceAddress)
	{
		var gameboy = TestUtils.InitGameboy();
		gameboy.cartridge.WriteByte((byte)(gameboy.cpu.PC + 1), sourceAddress);
		BaseTestLD(gameboy, new DataTypeU8Address(), new DataTypeReg8(destinationRegister), sourceValue);
	}

	public void TestLDReg8U8Address(CPURegister sourceRegister, byte sourceValue, byte destinationAddress)
	{
		var gameboy = TestUtils.InitGameboy();
		gameboy.cartridge.WriteByte((byte)(gameboy.cpu.PC + 1), destinationAddress);
		BaseTestLD(gameboy, new DataTypeReg8(sourceRegister), new DataTypeU8Address(), sourceValue);
	}

	public void TestLDU16Reg16(CPURegister destinationRegister, ushort sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), sourceValue);
		BaseTestLD(gameboy, new DataTypeU16(), new DataTypeReg16(destinationRegister), sourceValue);
	}

	public void TestLDReg16Reg16(CPURegister sourceRegister, CPURegister destinationRegister, ushort sourceValue)
	{
		var gameboy = TestUtils.InitGameboy();
		BaseTestLD(gameboy, new DataTypeReg16(sourceRegister), new DataTypeReg16(destinationRegister), sourceValue);
	}

	public override void Run()
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.A };

		foreach (var destinationRegister in registers)
			foreach (var sourceRegister in registers)
				TestLDReg8Reg8(sourceRegister, destinationRegister, 0x42);

		TestLDReg16AddressReg8(CPURegister.BC, CPURegister.A, 0x42);
		TestLDReg16AddressReg8(CPURegister.DE, CPURegister.A, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.A, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.B, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.C, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.D, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.E, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.H, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.L, 0x42);

		TestLDReg8Reg16Address(CPURegister.A, CPURegister.BC, 0x42);
		TestLDReg8Reg16Address(CPURegister.A, CPURegister.DE, 0x42);
		TestLDReg8Reg16Address(CPURegister.A, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.B, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.C, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.D, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.E, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.H, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.L, CPURegister.HL, 0x42);

		foreach (var destinationRegister in registers)
			TestLDU8Reg8(destinationRegister, 0x42);
		TestLDU8Reg16Address(CPURegister.HL, Memory.WorkRAMBank0.addr, 0x42);

		TestLDU16AddressReg8(CPURegister.A, Memory.WorkRAMBank0.addr, 0x42);
		TestLDReg8U16Address(CPURegister.A, Memory.WorkRAMBank0.addr, 0x42);

		TestLDReg8Reg8Address(CPURegister.A, CPURegister.C, 0x42, 0x69);
		TestLDReg8AddressReg8(CPURegister.C, CPURegister.A, 0x42, 0x69);

		TestLDReg8U8Address(CPURegister.A, 0x42, 0x69);
		TestLDU8AddressReg8(CPURegister.A, 0x42, 0x69);

		TestLDU16Reg16(CPURegister.BC, 0x4269);
		TestLDU16Reg16(CPURegister.DE, 0x4269);
		TestLDU16Reg16(CPURegister.HL, 0x4269);
		TestLDU16Reg16(CPURegister.SP, 0x4269);

		TestLDReg16Reg16(CPURegister.HL, CPURegister.SP, 0x4269);

		Console.WriteLine("All LD tests passed");
	}

	public void Expect(Gameboy gameboy, CPUOperator operation, bool condition)
	{
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " failed");
		Console.WriteLine(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " succeeded");
	}
}