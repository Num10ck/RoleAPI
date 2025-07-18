namespace RoleAPI.API.Managers
{
	using System.Collections.Generic;

	using Exiled.API.Features.Core.UserSettings;

	using Interfaces;

	public static class KeybindManager
	{
		private static IEnumerable<SettingBase> _settings;
		public static void RegisterKeybinds(IEnumerable<IAbility> abilities, string pluginName)
		{
			List<SettingBase> settings = new();
			
			var header = new HeaderSetting(
				name: $"Abilities of {pluginName}",
				hintDescription: $"Abilities of {pluginName}",
				paddling: true
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