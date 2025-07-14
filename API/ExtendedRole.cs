namespace RoleAPI.API
{
	using System.Collections.Generic;

	using Controller;

	using Exiled.API.Enums;
	using Exiled.API.Features;
	using Exiled.CustomRoles.API.Features;

	using Interfaces;

	using Managers;

	using PlayerRoles;

	using ProjectMER.Features.Objects;

	using UnityEngine;

	public abstract class ExtendedRole : CustomRole
	{
		public abstract List<EffectType> EffectList { get; set; }
		public abstract string SchematicName { get; set; }
		
		public static IReadOnlyDictionary<Player, ExtendedRole> Instances
		{
			get => _instances;
		}

		private static readonly Dictionary<Player, ExtendedRole> _instances = [];

		public SchematicObject Schematic { get; set; }

		public AudioPlayer AudioPlayer { get; set; }

		public Animator Animator { get; set; }

		public Speaker Speaker { get; set; }
		
		public HintController HintController { get; set; }

		public override void AddRole(Player player)
		{
			player.Role.Set(this.Role, SpawnReason.ForceClass, RoleSpawnFlags.None);
			player.Position = SpawnProperties.DynamicSpawnPoints.RandomItem().Position;
        
			player.ClearItems();
			player.ClearAmmo();
			player.UniqueRole = this.Name;
			this.TrackedPlayers.Add(player);
			player.Health = this.MaxHealth;
			player.MaxHealth = this.MaxHealth;
			player.Scale = this.Scale;
			player.CustomName = this.Name;
			player.CustomInfo = player.CustomName + "\n" + this.CustomInfo;
			
			foreach (EffectType effectType in this.EffectList) //todo add intensity
			{
				player.EnableEffect(effectType);
			}
			
			this.ShowMessage(player);
			this.ShowBroadcast(player);
			this.RoleAdded(player);
			player.SendConsoleMessage(this.ConsoleMessage, "green");
			
			_instances.Add(player, this);
		}

		public override void RemoveRole(Player player)
		{
			base.RemoveRole(player);
			player.CustomName = null;
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
			role.AudioPlayer = player.AddAudio(volume);

			if (!role.AudioPlayer.TryGetSpeaker("scp999-speaker", out Speaker speaker))
			{
				Log.Error("Failed to get Speaker from custom role.");

				return;
			}
			
			role.Speaker = speaker;

			role.Animator = SchematicManager.GetAnimatorFromSchematic(schematic);

			//role.HintController = GameObject.AddComponent<HintController>();
			//role.HintController.Init(player);

			// NOTE: Local position excluded.
			role.Schematic.transform.SetParent(player.Transform);
			role.Speaker.transform.SetParent(role.Schematic.transform);
		}

		private static void Destroy(Player player, ExtendedRole role)
		{
			//GameObject.Destroy(role.HintController);

			role.AudioPlayer.RemoveAllClips();
			role.AudioPlayer.Destroy();

			role.Schematic.Destroy();
		}
	}
}
