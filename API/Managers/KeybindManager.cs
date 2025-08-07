namespace RoleAPI.API.Managers
{
	using System;
	using System.Collections.Generic;

	using Exiled.API.Features.Core.UserSettings;

	using Interfaces;

	public static class KeybindManager
	{
		private static IEnumerable<SettingBase> _settings;
		public static void RegisterKeybinds(string pluginName, int pluginId, IAbility[] abilities)
		{
			List<SettingBase> settings = new();
			
			var header = new HeaderSetting(
				id: pluginId,
				name: $"Abilities of {pluginName}",
				padding: true
			);
			
			settings.Add(header);

			foreach (IAbility ability in abilities)
			{
				KeybindSetting setting = new(
					ability.KeyId,
					$"{ability.Name} - {ability.Description}",
					ability.KeyCode,
					false,
					false,
					ability.Description
				);

				settings.Add(setting);
			}

			_settings = settings;
        
			SettingBase.Register(_settings);
			SettingBase.SendToAll();
		}

		public static void UnregisterKeybinds()
		{
			SettingBase.Unregister(settings: _settings);
		}
	}
}