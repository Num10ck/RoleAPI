namespace RoleAPI
{
	using System.IO;

	using Exiled.API.Features;

	using HarmonyLib;

	public static class Startup
	{
		public static string SchematicPath;
		
		public static string AudioPath;
		
		public static void SetupAPI(string pluginName)
		{
			// Patch
			Harmony harmony = new Harmony($"risottoman.{pluginName}");
			harmony.PatchAll();
			
			// Path
			string basePath = Path.Combine(Paths.IndividualConfigs, pluginName.ToLower());
			SchematicPath = Path.Combine(basePath, "Schematics");
			AudioPath = Path.Combine(basePath, "Audio");
			CreatePluginDirectory(SchematicPath);
			CreatePluginDirectory(AudioPath);
		
			// Load audio files
			API.Managers.AudioExtensions.LoadAudioFiles(AudioPath);
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