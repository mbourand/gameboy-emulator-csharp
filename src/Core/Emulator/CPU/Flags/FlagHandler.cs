using System.Collections.Generic;

namespace GBMU.Core;

public class FlagHandler
{
	private Dictionary<CPUFlag, FlagAuthorization> _authorizations;

	public FlagHandler(Dictionary<CPUFlag, FlagAuthorization> authorizations) => _authorizations = authorizations;

	public void Apply(CPU cpu, Dictionary<CPUFlag, bool> tryValues)
	{
		foreach (var flag in _authorizations.Keys)
			TrySetFlag(cpu, flag, tryValues.ContainsKey(flag) ? tryValues[flag] : false);
	}

	private void TrySetFlag(CPU cpu, CPUFlag flag, bool value)
	{
		switch (_authorizations[flag])
		{
			case FlagAuthorization.UNSET:
				cpu.SetFlag(flag, false);
				break;
			case FlagAuthorization.SET:
				cpu.SetFlag(flag, true);
				break;
			case FlagAuthorization.USE:
				cpu.SetFlag(flag, value);
				break;
			case FlagAuthorization.IGNORE:
				break;
		}
	}
}