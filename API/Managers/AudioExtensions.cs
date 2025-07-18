namespace RoleAPI.API.Managers
{
	using System.IO;

	using Configs;

	using Exiled.API.Features;

	public static class AudioExtensions
	{
		public static AudioPlayer AddAudio(this Player player, AudioConfig config)
		{
			AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"{config.Name} {player.Nickname}", onIntialCreation: (p) =>
			{
				// Attach created audio player to player.
				p.transform.parent = player.GameObject.transform;

				// This created speaker will be in 3D space.
				p.AddSpeaker(
					$"{config.Name}-speaker", 
					config.Volume, 
					config.IsSpatial, 
					config.MinDistance, 
					config.MaxDistance);
			});

			return audioPlayer;
		}

		public static void LoadAudioFiles(string path)
		{
			if (!Directory.Exists(path))
			{
				Log.Error($"Directory \"{path}\" isn't exist.");
			}

			foreach (string name in Directory.EnumerateFiles(path))
			{
				if (!AudioClipStorage.AudioClips.ContainsKey(name) && name.EndsWith(".ogg"))
				{
					if (!AudioClipStorage.LoadClip(name))
					{
						Log.Error($"[LoadAudioFiles] The audio file {name} was not found for playback.");
					}
				}
			}
		}
	}
}