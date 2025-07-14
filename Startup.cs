namespace RoleAPI
{
	using System.IO;

	using Exiled.API.Features;

	using HarmonyLib;

	public static class Startup
	{
		public static string AssemblyName;
		public static string SchematicPath;
		public static string AudioPath;
		public static void SetupAPI(string pluginName, string assemblyName)
		{
			AssemblyName = assemblyName;
			
			// Patch
			var harmony = new Harmony($"risottoman.{pluginName}");
			harmony.PatchAll();
			
			// Path
			string basePath = Path.Combine(Paths.IndividualConfigs, pluginName.ToLower());
			SchematicPath = Path.Combine(basePath, "Schematics");
			AudioPath = Path.Combine(basePath, "Audio");
			CreatePluginDirectory(SchematicPath);
			CreatePluginDirectory(AudioPath);
		
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