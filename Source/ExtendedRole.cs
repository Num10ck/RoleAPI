namespace CustomRoles
{
	using System.Collections.Generic;

	using CustomRoles.Features.Managers;
	using CustomRoles.Interfaces;

	using Exiled.API.Features;
	using Exiled.API.Features.Core.UserSettings;
	using Exiled.CustomRoles.API.Features;

	using ProjectMER.Features.Objects;

	using UnityEngine;

	public abstract class ExtendedRole : CustomRole
	{
		public static IReadOnlyDictionary<Player, ExtendedRole> Instances
		{
			get => _instances;
		}

		private readonly static Dictionary<Player, ExtendedRole> _instances = [];

		public SchematicObject Schematic { get; set; }

		public AudioPlayer Audio { get; set; }

		public Animator Animator { get; set; }

		public Speaker Speaker { get; set; }

		public override void AddRole(Player player)
		{
			_instances.Add(player, this);
		}

		public override void RemoveRole(Player player)
		{
			_instances.Remove(player);
		}

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
			ExtendedRole role,
			SchematicObject schematic,
			IEnumerable<IAbility> abilities,
			int volume = 100)
		{
			role.Audio = AudioExtensions.AddAudio(player, volume);

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

			// NOTE: Local position excluded.
			role.Schematic.transform.SetParent(player.Transform);
			role.Speaker.transform.SetParent(role.Schematic.transform);
		}

		public static void Destroy(Player player, ExtendedRole role)
		{
			InvisibilityExtensions.RemoveInvisibility(player);
			KeybindManager.UnregisterKeybindsForPlayer(player);
			HintExtensions.RemoveHint(player);

			role.Audio.RemoveAllClips();
			role.Audio.Destroy();

			role.Schematic.Destroy();
		}
	}
}
