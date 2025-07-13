namespace RoleAPI
{
	using System.IO;

	using Exiled.API.Features;

	using HarmonyLib;

	public class Startup
	{
		public static void SetupAPI(string pluginName)
		{
			// Patch
			var harmony = new Harmony($"risottoman.{pluginName}");
			harmony.PatchAll();
		
			string basePath = Path.Combine(Paths.IndividualConfigs, pluginName.ToLower());
			CreatePluginDirectory(Path.Combine(basePath, "Schematics"));
			CreatePluginDirectory(Path.Combine(basePath, "Audio"));
		
			// Register the abilities
			API.Managers.AbilityRegistrator.RegisterAbilities();
			API.Managers.KeybindManager.RegisterKeybinds(
				API.Managers.AbilityRegistrator.GetAbilities,
				pluginName
			);
		}
		
		private static void CreatePluginDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}