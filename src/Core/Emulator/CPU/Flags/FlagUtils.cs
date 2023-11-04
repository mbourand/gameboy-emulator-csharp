using System.Collections.Generic;

namespace GBMU.Core;

public static class FlagUtils
{
	// Maps a string like "Z0H-" to a dict
	public static Dictionary<CPUFlag, FlagPermission> PermissionsFromString(string input)
	{
		var indexToFlag = new CPUFlag[] { CPUFlag.Zero, CPUFlag.NSubtract, CPUFlag.HalfCarry, CPUFlag.Carry };
		var charToPermissions = new Dictionary<char, FlagPermission>()
		{
			{ '-', FlagPermission.Ignore },
			{ '0', FlagPermission.Unset },
			{ '1', FlagPermission.Set },
		};

		var result = new Dictionary<CPUFlag, FlagPermission>();
		for (var i = 0; i < input.Length; i++)
		{
			var flag = indexToFlag[i];
			if (charToPermissions.ContainsKey(input[i]))
				result[flag] = charToPermissions[input[i]];
			else
				result[flag] = FlagPermission.Use;
		}

		return result;
	}

	public static string FlagConditionToMnemonic(CPUFlag flag, bool expectedValue)
	{
		var negativePrefix = expectedValue ? "" : "N";
		var flagChar = flag.GetChar();
		return $"{negativePrefix}{flagChar}";
	}

	public static char GetChar(this CPUFlag flag) => flag switch
	{
		CPUFlag.Zero => 'Z',
		CPUFlag.NSubtract => 'N',
		CPUFlag.HalfCarry => 'H',
		CPUFlag.Carry => 'C',
		_ => throw new System.Exception("Invalid flag")
	};
}