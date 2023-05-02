using GBMU.Core;

namespace GBMU;

public static class InstructionSet
{
	public static CPUOperator GetInstruction(byte opcode) => opcode switch
	{
		0x00 => new OperatorNOP(),
		0x01 => new OperatorLoad(new DataTypeU16(), new DataTypeReg16(CPURegister.BC)),
		0x02 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg16Address(CPURegister.BC)),
		0x03 => new OperatorINC(new DataTypeReg16(CPURegister.BC)),
		0x04 => new OperatorINC(new DataTypeReg8(CPURegister.B)),
		0x05 => new OperatorDEC(new DataTypeReg8(CPURegister.B)),
		0x06 => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.B)),
		0x07 => new OperatorRLCA(),
		0x08 => new OperatorLoad(new DataTypeReg16Address(CPURegister.SP), new DataTypeU16Address()),
		0x09 => new OperatorADD(new DataTypeReg16(CPURegister.BC), new DataTypeReg16(CPURegister.HL), new FlagPermissionHandler("-0HC"), CarryBit.BIT_16, HalfCarryBit.BIT_12),
		0x0A => new OperatorLoad(new DataTypeReg16Address(CPURegister.BC), new DataTypeReg8(CPURegister.A)),
		0x0B => new OperatorDEC(new DataTypeReg16(CPURegister.BC)),
		0x0C => new OperatorINC(new DataTypeReg8(CPURegister.C)),
		0x0D => new OperatorDEC(new DataTypeReg8(CPURegister.C)),
		0x0E => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.C)),
		0x0F => new OperatorRRCA(),

		// 0x10 => new OperatorSTOP(),
		0x11 => new OperatorLoad(new DataTypeU16(), new DataTypeReg16(CPURegister.DE)),
		0x12 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg16Address(CPURegister.DE)),
		0x13 => new OperatorINC(new DataTypeReg16(CPURegister.DE)),
		0x14 => new OperatorINC(new DataTypeReg8(CPURegister.D)),
		0x15 => new OperatorDEC(new DataTypeReg8(CPURegister.D)),
		0x16 => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.D)),
		0x17 => new OperatorRLA(),
		0x18 => new OperatorJR(new DataTypeU8()),
		0x19 => new OperatorADD(new DataTypeReg16(CPURegister.DE), new DataTypeReg16(CPURegister.HL), new FlagPermissionHandler("-0HC"), CarryBit.BIT_16, HalfCarryBit.BIT_12),
		0x1A => new OperatorLoad(new DataTypeReg16Address(CPURegister.DE), new DataTypeReg8(CPURegister.A)),
		0x1B => new OperatorDEC(new DataTypeReg16(CPURegister.DE)),
		0x1C => new OperatorINC(new DataTypeReg8(CPURegister.E)),
		0x1D => new OperatorDEC(new DataTypeReg8(CPURegister.E)),
		0x1E => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.E)),
		0x1F => new OperatorRRA(),

		0x20 => new OperatorJRFlag(CPUFlag.ZERO, false, new DataTypeU8()),
		0x21 => new OperatorLoad(new DataTypeU16(), new DataTypeReg16(CPURegister.HL)),
		0x22 => new OperatorLoadShift(new DataTypeReg8(CPURegister.A), new DataTypeReg16Address(CPURegister.HL), CPURegister.HL, ShiftingBehaviour.INCREMENT),
		0x23 => new OperatorINC(new DataTypeReg16(CPURegister.HL)),
		0x24 => new OperatorINC(new DataTypeReg8(CPURegister.H)),
		0x25 => new OperatorDEC(new DataTypeReg8(CPURegister.H)),
		0x26 => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.H)),
		0x27 => new OperatorDAA(),
		0x28 => new OperatorJRFlag(CPUFlag.ZERO, true, new DataTypeU8()),
		0x29 => new OperatorADD(new DataTypeReg16(CPURegister.HL), new DataTypeReg16(CPURegister.HL), new FlagPermissionHandler("-0HC"), CarryBit.BIT_16, HalfCarryBit.BIT_12),
		0x2A => new OperatorLoadShift(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), CPURegister.HL, ShiftingBehaviour.INCREMENT),
		0x2B => new OperatorDEC(new DataTypeReg16(CPURegister.HL)),
		0x2C => new OperatorINC(new DataTypeReg8(CPURegister.L)),
		0x2D => new OperatorDEC(new DataTypeReg8(CPURegister.L)),
		0x2E => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.L)),
		0x2F => new OperatorCPL(),

		0x30 => new OperatorJRFlag(CPUFlag.CARRY, false, new DataTypeU8()),
		0x31 => new OperatorLoad(new DataTypeU16(), new DataTypeReg16(CPURegister.SP)),
		0x32 => new OperatorLoadShift(new DataTypeReg8(CPURegister.A), new DataTypeReg16Address(CPURegister.HL), CPURegister.HL, ShiftingBehaviour.DECREMENT),
		0x33 => new OperatorINC(new DataTypeReg16(CPURegister.SP)),
		0x34 => new OperatorINC(new DataTypeReg16Address(CPURegister.HL)),
		0x35 => new OperatorDEC(new DataTypeReg16Address(CPURegister.HL)),
		0x36 => new OperatorLoad(new DataTypeU8(), new DataTypeReg16Address(CPURegister.HL)),
		0x37 => new OperatorSCF(),
		0x38 => new OperatorJRFlag(CPUFlag.CARRY, true, new DataTypeU8()),
		0x39 => new OperatorADD(new DataTypeReg16(CPURegister.SP), new DataTypeReg16(CPURegister.HL), new FlagPermissionHandler("-0HC"), CarryBit.BIT_16, HalfCarryBit.BIT_12),
		0x3A => new OperatorLoadShift(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), CPURegister.HL, ShiftingBehaviour.DECREMENT),
		0x3B => new OperatorDEC(new DataTypeReg16(CPURegister.SP)),
		0x3C => new OperatorINC(new DataTypeReg8(CPURegister.A)),
		0x3D => new OperatorDEC(new DataTypeReg8(CPURegister.A)),
		0x3E => new OperatorLoad(new DataTypeU8(), new DataTypeReg8(CPURegister.A)),
		0x3F => new OperatorCCF(),

		0x40 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.B)),
		0x41 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.B)),
		0x42 => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.B)),
		0x43 => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.B)),
		0x44 => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.B)),
		0x45 => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.B)),
		0x46 => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.B)),
		0x47 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.B)),
		0x48 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.C)),
		0x49 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.C)),
		0x4A => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.C)),
		0x4B => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.C)),
		0x4C => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.C)),
		0x4D => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.C)),
		0x4E => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.C)),
		0x4F => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.C)),

		0x50 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.D)),
		0x51 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.D)),
		0x52 => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.D)),
		0x53 => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.D)),
		0x54 => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.D)),
		0x55 => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.D)),
		0x56 => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.D)),
		0x57 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.D)),
		0x58 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.E)),
		0x59 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.E)),
		0x5A => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.E)),
		0x5B => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.E)),
		0x5C => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.E)),
		0x5D => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.E)),
		0x5E => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.E)),
		0x5F => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.E)),

		0x60 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.H)),
		0x61 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.H)),
		0x62 => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.H)),
		0x63 => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.H)),
		0x64 => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.H)),
		0x65 => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.H)),
		0x66 => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.H)),
		0x67 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.H)),
		0x68 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.L)),
		0x69 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.L)),
		0x6A => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.L)),
		0x6B => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.L)),
		0x6C => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.L)),
		0x6D => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.L)),
		0x6E => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.L)),
		0x6F => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.L)),

		0x70 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg16Address(CPURegister.HL)),
		0x71 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg16Address(CPURegister.HL)),
		0x72 => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg16Address(CPURegister.HL)),
		0x73 => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg16Address(CPURegister.HL)),
		0x74 => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg16Address(CPURegister.HL)),
		0x75 => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg16Address(CPURegister.HL)),
		// 0x76 => new OperatorHalt(),
		0x77 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg16Address(CPURegister.HL)),
		0x78 => new OperatorLoad(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A)),
		0x79 => new OperatorLoad(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A)),
		0x7A => new OperatorLoad(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A)),
		0x7B => new OperatorLoad(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A)),
		0x7C => new OperatorLoad(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A)),
		0x7D => new OperatorLoad(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A)),
		0x7E => new OperatorLoad(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A)),
		0x7F => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A)),

		0x80 => new OperatorADD(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x81 => new OperatorADD(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x82 => new OperatorADD(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x83 => new OperatorADD(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x84 => new OperatorADD(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x85 => new OperatorADD(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x86 => new OperatorADD(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x87 => new OperatorADD(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x88 => new OperatorADC(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x89 => new OperatorADC(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x8A => new OperatorADC(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x8B => new OperatorADC(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x8C => new OperatorADC(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x8D => new OperatorADC(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x8E => new OperatorADC(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x8F => new OperatorADC(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),

		0x90 => new OperatorSUB(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x91 => new OperatorSUB(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x92 => new OperatorSUB(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x93 => new OperatorSUB(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x94 => new OperatorSUB(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x95 => new OperatorSUB(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x96 => new OperatorSUB(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0x97 => new OperatorSUB(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		// SBC

		0xA0 => new OperatorAND(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA1 => new OperatorAND(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA2 => new OperatorAND(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA3 => new OperatorAND(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA4 => new OperatorAND(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA5 => new OperatorAND(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA6 => new OperatorAND(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA7 => new OperatorAND(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xA8 => new OperatorXOR(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xA9 => new OperatorXOR(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xAA => new OperatorXOR(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xAB => new OperatorXOR(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xAC => new OperatorXOR(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xAD => new OperatorXOR(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xAE => new OperatorXOR(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xAF => new OperatorXOR(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),

		0xB0 => new OperatorOR(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB1 => new OperatorOR(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB2 => new OperatorOR(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB3 => new OperatorOR(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB4 => new OperatorOR(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB5 => new OperatorOR(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB6 => new OperatorOR(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB7 => new OperatorOR(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xB8 => new OperatorCP(new DataTypeReg8(CPURegister.B), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xB9 => new OperatorCP(new DataTypeReg8(CPURegister.C), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xBA => new OperatorCP(new DataTypeReg8(CPURegister.D), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xBB => new OperatorCP(new DataTypeReg8(CPURegister.E), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xBC => new OperatorCP(new DataTypeReg8(CPURegister.H), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xBD => new OperatorCP(new DataTypeReg8(CPURegister.L), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xBE => new OperatorCP(new DataTypeReg16Address(CPURegister.HL), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),
		0xBF => new OperatorCP(new DataTypeReg8(CPURegister.A), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),

		0xC2 => new OperatorJPFlag(CPUFlag.ZERO, false, new DataTypeU16()),
		0xC3 => new OperatorJP(new DataTypeU16()),
		0xC6 => new OperatorADD(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0xCA => new OperatorJPFlag(CPUFlag.ZERO, true, new DataTypeU16()),
		0xCE => new OperatorADC(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z0HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),

		0xD2 => new OperatorJPFlag(CPUFlag.CARRY, false, new DataTypeU16()),
		0xD6 => new OperatorSUB(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC"), CarryBit.BIT_8, HalfCarryBit.BIT_4),
		0xDA => new OperatorJPFlag(CPUFlag.CARRY, true, new DataTypeU16()),

		0xE0 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeU8Address()),
		0xE2 => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeReg8Address(CPURegister.C)),
		0xE6 => new OperatorAND(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z010")),
		0xE8 => new OperatorADD(new DataTypeU8(), new DataTypeReg16(CPURegister.SP), new FlagPermissionHandler("00HC"), CarryBit.BIT_16, HalfCarryBit.BIT_12, true),
		0xE9 => new OperatorJP(new DataTypeReg16Address(CPURegister.HL)),
		0xEA => new OperatorLoad(new DataTypeReg8(CPURegister.A), new DataTypeU16Address()),
		0xEE => new OperatorXOR(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),

		0xF0 => new OperatorLoad(new DataTypeU8Address(), new DataTypeReg8(CPURegister.A)),
		0xF2 => new OperatorLoad(new DataTypeReg8Address(CPURegister.C), new DataTypeReg8(CPURegister.A)),
		0xF6 => new OperatorOR(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z000")),
		0xFA => new OperatorLoad(new DataTypeU16Address(), new DataTypeReg8(CPURegister.A)),
		0xFE => new OperatorCP(new DataTypeU8(), new DataTypeReg8(CPURegister.A), new FlagPermissionHandler("Z1HC")),


		_ => throw new System.Exception("Invalid opcode")
	};
}