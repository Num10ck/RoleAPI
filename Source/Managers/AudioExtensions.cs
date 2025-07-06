namespace CustomRoles.Features.Managers
{
	using System.Collections.Generic;
	using System.IO;

	using Exiled.API.Features;

	public static class AudioExtensions
	{
		private static List<string> _audioNameList = new()
		{
			"yippee-tbh1",
			"yippee-tbh2",
			"hello",
			"hi",
			"jump",
			"health",
			"circus",
			"funnytoy",
			"uwu"
		};

		public static AudioPlayer AddAudioPlayer(this Player player, int volume)
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

		private static void LoadAudioFiles()
		{
			string path = Plugin.Singleton.AudioPath;

			foreach (string audioName in _audioNameList)
			{
				if (!AudioClipStorage.AudioClips.ContainsKey(audioName))
				{
					string filePath = Path.Combine(path, audioName) + ".ogg";

					if (!AudioClipStorage.LoadClip(filePath, audioName))
					{
						Log.Error($"[AddAudioPlayer] The audio file {audioName} was not found for playback");
					}
				}
			}
		}

		static AudioExtensions()
		{
			LoadAudioFiles();
		}
	}
}