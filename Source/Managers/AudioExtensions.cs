namespace CustomRoles.Features.Managers
{
	using System.IO;

	using Exiled.API.Features;

	public static class AudioExtensions
	{
		public static AudioPlayer AddAudio(this Player player, int volume)
		{
			AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Scp999 {player.Nickname}", onIntialCreation: (p) =>
			{
				// Attach created audio player to player.
				p.transform.parent = player.GameObject.transform;

				// This created speaker will be in 3D space.
				Speaker speaker = p.AddSpeaker("scp999-speaker", volume, true, 5f, 15f);
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
						Log.Error($"[{nameof(LoadAudioFiles)}] The audio file {name} was not found for playback.");
					}
				}
			}
		}
	}
}