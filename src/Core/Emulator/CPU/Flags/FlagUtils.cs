using System.Collections.Generic;

namespace GBMU.Core;

public static class FlagUtils
{
	// Maps a string like "Z0H-" to a dict
	public static Dictionary<CPUFlag, FlagPermission> PermissionsFromString(string input)
	{
		var indexToFlag = new CPUFlag[] { CPUFlag.ZERO, CPUFlag.N_SUBTRACT, CPUFlag.HALF_CARRY, CPUFlag.CARRY };
		var charToPermissions = new Dictionary<char, FlagPermission>()
		{
			{ '-', FlagPermission.IGNORE },
			{ '0', FlagPermission.UNSET },
			{ '1', FlagPermission.SET },
		};

		var result = new Dictionary<CPUFlag, FlagPermission>();
		for (var i = 0; i < input.Length; i++)
		{
			var flag = indexToFlag[i];
			if (charToPermissions.ContainsKey(input[i]))
				result[flag] = charToPermissions[input[i]];
			else
				result[flag] = FlagPermission.USE;
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
		CPUFlag.ZERO => 'Z',
		CPUFlag.N_SUBTRACT => 'N',
		CPUFlag.HALF_CARRY => 'H',
		CPUFlag.CARRY => 'C',
		_ => throw new System.Exception("Invalid flag")
	};
}