namespace RoleAPI
{
	using System;
	using System.IO;
	using System.Linq;

	using Exiled.API.Features;

	public static class Startup
	{
		public static string AudioPath;
		
		public static bool SetupAPI(string pluginName)
		{
			// Checking that the ProjectMER plugin is loaded on the server
			if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.ToLower().Contains("projectmer")))
			{
				Log.Error("ProjectMER is not installed. Schematics can't spawn the game model.");
				return false;
			}
			
			// Path
			string basePath = Path.Combine(Paths.IndividualConfigs, pluginName.ToLower());
			string schematicPath = Path.Combine(basePath, "Schematics");
			AudioPath = Path.Combine(basePath, "Audio");
			RemovePluginDirectory(schematicPath);
			CreatePluginDirectory(AudioPath);
			
			// Load audio files
			API.Managers.AudioExtensions.LoadAudioFiles(AudioPath);

			return true;
		}
		
		private static void CreatePluginDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		private static void RemovePluginDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
				Log.Error("The Schematics directory will be deleted. Move the schematics to ProjectMER.");
			}
		}
	}
}