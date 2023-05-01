using System.Collections.Generic;

namespace GBMU.Core;

public enum FlagAuthorization
{
	UNSET,
	SET,
	IGNORE,
	USE,
}

public static class FlagAuthorizationUtils
{
	// Maps a string like "Z0H-" to a dict
	public static Dictionary<CPUFlag, FlagAuthorization> AuthorizationsFromString(string input)
	{
		var indexToFlag = new CPUFlag[] { CPUFlag.ZERO, CPUFlag.N_SUBTRACT, CPUFlag.HALF_CARRY, CPUFlag.CARRY };
		var charToAuthorization = new Dictionary<char, FlagAuthorization>()
		{
			{ '-', FlagAuthorization.IGNORE },
			{ '0', FlagAuthorization.UNSET },
			{ '1', FlagAuthorization.SET },
		};

		var result = new Dictionary<CPUFlag, FlagAuthorization>();
		for (var i = 0; i < input.Length; i++)
		{
			var flag = indexToFlag[i];
			if (charToAuthorization.ContainsKey(input[i]))
				result[flag] = charToAuthorization[input[i]];
			else
				result[flag] = FlagAuthorization.USE;
		}

		return result;
	}
}