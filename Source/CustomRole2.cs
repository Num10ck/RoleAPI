namespace CustomRoles
{
	using System.Collections.Generic;

	using Exiled.API.Features;
	using Exiled.API.Features.Core.UserSettings;

	using ProjectMER.Features.Objects;

	using CustomRoles.Features.Controller;

	using CustomRoles.Features.Managers;
	using CustomRoles.Interfaces;

	using UnityEngine;
	using Exiled.CustomRoles.API.Features;

	public abstract class CustomRole2 : CustomRole
	{
		public SchematicObject Schematic { get; set; }

		public AudioPlayer Audio { get; set; }

		public Animator Animator { get; set; }

		public Speaker Speaker { get; set; }

		protected MovementController MovementController { get; set; }

		public void Create(
			Player player,
			SchematicObject schematic,
			IEnumerable<IAbility> abilities,
			int volume = 100)
		{
			Create(player, this, schematic, abilities, volume);
		}

		public void Destroy(Player player)
		{
			Destroy(player, this);
		}

		public static void Create(
			Player player,
			CustomRole2 role,
			SchematicObject schematic,
			IEnumerable<IAbility> abilities,
			int volume = 100)
		{
			role.Audio = AudioExtensions.AddAudioPlayer(player, volume);

			if (!role.Audio.TryGetSpeaker("scp999-speaker", out Speaker speaker))
			{
				Log.Error("Failed to get Speaker from custom role.");

				return;
			}

			role.Speaker = speaker;

			role.Animator = SchematicManager.GetAnimatorFromSchematic(schematic);

			HeaderSetting setting = new(
				"Custom role",
				"Abilities"
			);

			KeybindManager.RegisterKeybindsForPlayer(player, setting, abilities);
			HintExtensions.AddHint(player);
			InvisibilityExtensions.MakeInvisible(player);

			role.MovementController = player.GameObject.AddComponent<MovementController>();

			role.MovementController.Init(
				role.Schematic,
				role.Speaker,
				role.Schematic.transform.localPosition);
		}

		public static void Destroy(Player player, CustomRole2 role)
		{
			GameObject.Destroy(role.MovementController);

			InvisibilityExtensions.RemoveInvisibility(player);
			KeybindManager.UnregisterKeybindsForPlayer(player);
			HintExtensions.RemoveHint(player);

			role.Audio.RemoveAllClips();
			role.Audio.Destroy();

			role.Schematic.Destroy();
		}
	}
}
