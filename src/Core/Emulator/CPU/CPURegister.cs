namespace GBMU.Core;

public enum CPURegister {
	A,
	B,
	C,
	D,
	E,
	H,
	L,
	F,
	AF,
	BC,
	DE,
	HL,
	SP,
	PC,
}

public partial class CPU {
	public ushort Get16BitRegister(CPURegister register) {
		return register switch {
			CPURegister.AF => AF,
			CPURegister.BC => BC,
			CPURegister.DE => DE,
			CPURegister.HL => HL,
			CPURegister.SP => SP,
			CPURegister.PC => PC,
			_ => throw new System.Exception("Invalid register")
		};
	}

	public void Set16BitRegister(CPURegister register, ushort value) {
		switch (register) {
			case CPURegister.AF:
				AF = (ushort)(value & 0xFFF0); // Lower 4 bits of F can't be set
				break;
			case CPURegister.BC:
				BC = value;
				break;
			case CPURegister.DE:
				DE = value;
				break;
			case CPURegister.HL:
				HL = value;
				break;
			case CPURegister.SP:
				SP = value;
				break;
			case CPURegister.PC:
				PC = value;
				break;
			default:
				throw new System.Exception("Invalid register");
		}
	}

	public byte Get8BitRegister(CPURegister register) {
		return register switch {
			CPURegister.A => A,
			CPURegister.B => B,
			CPURegister.C => C,
			CPURegister.D => D,
			CPURegister.E => E,
			CPURegister.H => H,
			CPURegister.L => L,
			_ => throw new System.Exception("Invalid register")
		};
	}

	public void Set8BitRegister(CPURegister register, byte value) {
		switch (register) {
			case CPURegister.A:
				A = value;
				break;
			case CPURegister.B:
				B = value;
				break;
			case CPURegister.C:
				C = value;
				break;
			case CPURegister.D:
				D = value;
				break;
			case CPURegister.E:
				E = value;
				break;
			case CPURegister.H:
				H = value;
				break;
			case CPURegister.L:
				L = value;
				break;
			default:
				throw new System.Exception("Invalid register");
		}
	}
}