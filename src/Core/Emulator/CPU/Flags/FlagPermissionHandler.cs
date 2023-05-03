using System.Collections.Generic;

namespace GBMU.Core;

public class FlagPermissionHandler
{
	private Dictionary<CPUFlag, FlagPermission> _permissions;

	public FlagPermissionHandler(Dictionary<CPUFlag, FlagPermission> permissions) => _permissions = permissions;
	public FlagPermissionHandler(string permissions) => _permissions = FlagUtils.PermissionsFromString(permissions);

	public void Apply(CPU cpu, Dictionary<CPUFlag, bool> tryValues)
	{
		foreach (var flag in _permissions.Keys)
			TrySetFlag(cpu, flag, tryValues.ContainsKey(flag) ? tryValues[flag] : false);
	}

	private void TrySetFlag(CPU cpu, CPUFlag flag, bool value)
	{
		switch (_permissions[flag])
		{
			case FlagPermission.UNSET:
				cpu.SetFlag(flag, false);
				break;
			case FlagPermission.SET:
				cpu.SetFlag(flag, true);
				break;
			case FlagPermission.USE:
				cpu.SetFlag(flag, value);
				break;
			case FlagPermission.IGNORE:
				break;
		}
	}
}