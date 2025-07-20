namespace RoleAPI.API
{
	using System.Collections.Generic;

	using Configs;

	using Controller;

	using Exiled.API.Enums;
	using Exiled.API.Extensions;
	using Exiled.API.Features;
	using Exiled.CustomRoles.API.Features;

	using LabApi.Events.Arguments.PlayerEvents;
	using LabApi.Features.Wrappers;

	using Managers;

	using MEC;

	using PlayerRoles;

	using ProjectMER.Features.Objects;

	using UnityEngine;

	using YamlDotNet.Serialization;

	using Player = Exiled.API.Features.Player;

	public abstract class ExtendedRole : CustomRole
	{
		public static IReadOnlyDictionary<Player, ExtendedRole> Instances
		{
			get => _instances;
		}
		
		private static readonly Dictionary<Player, ExtendedRole> _instances = [];
		
		public abstract SpawnConfig SpawnConfig { get; set; }
		
		public abstract SchematicConfig SchematicConfig { get; set; }
		
		public abstract TextToyConfig TextToyConfig { get; set; }
		
		public abstract HintConfig HintConfig { get; set; }
		
		public abstract AudioConfig AudioConfig { get; set; }
		
		public abstract List<EffectConfig> Effects { get; set; }
		
		public abstract bool IsPlayerInvisible { get; set; }
		
		public abstract bool IsShowPlayerNickname { get; set; }
		
		[YamlIgnore]
		private SchematicObject SchematicObject { get; set; }

		[YamlIgnore]
		public AudioPlayer AudioPlayer { get; set; }
		
		[YamlIgnore]
		public Animator Animator { get; set; }

		[YamlIgnore]
		public Speaker Speaker { get; set; }
		
		[YamlIgnore]
		private TextToy TextToy { get; set; }
		
		[YamlIgnore]
		private HintController HintController { get; set; }
		
		[YamlIgnore]
		private CooldownController CooldownController { get; set; }

		protected override void SubscribeEvents()
		{
			base.SubscribeEvents();
			Exiled.Events.Handlers.Server.RoundStarted += this.OnRoundStarted;
			LabApi.Events.Handlers.PlayerEvents.ValidatedVisibility += this.OnPlayerValidatedVisibility;
		}

		protected override void UnsubscribeEvents()
		{
			Exiled.Events.Handlers.Server.RoundStarted -= this.OnRoundStarted;
			LabApi.Events.Handlers.PlayerEvents.ValidatedVisibility -= this.OnPlayerValidatedVisibility;
			base.UnsubscribeEvents();
		}
		
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
			if (this.IsShowPlayerNickname is true)
			{
				player.CustomName = $"{this.Name} {player.Nickname}";
			}
			
			player.CustomInfo = player.CustomName + "\n" + this.CustomInfo;
			
			foreach (EffectConfig effect in this.Effects)
			{
				player.EnableEffect(
					type: effect.EffectType,
					intensity: effect.Intensity
				);
			}
			
			this.ShowBroadcast(player);
			this.RoleAdded(player);
			player.SendConsoleMessage(this.ConsoleMessage, "green");
			
			_instances.Add(player, this);
			
			this.CreateObjects(player);
		}

		public override void RemoveRole(Player player)
		{
			this.DestroyObjects();
			
			player.CustomName = null;
			_instances.Remove(player);
			
			base.RemoveRole(player);
		}

		private void CreateObjects(Player player)
		{
			// Create SchematicObject from SchematicName
			this.SchematicObject = SchematicManager.SpawnSchematic(this.SchematicConfig);
			if (this.SchematicObject is null)
			{
				Log.Error("Failed to create Schematic for custom role.");
				return;
			}
			
			// Create AudioPlayer for player
			if (this.AudioConfig.Volume > 0)
			{
				this.AudioPlayer = player.AddAudio(this.AudioConfig);
				if (!this.AudioPlayer.TryGetSpeaker($"{this.AudioConfig.Name}-speaker", out Speaker speaker))
				{
					Log.Error("Failed to get Speaker from custom role.");
					return;
				}
				
				// Attach speaker to schematic
				this.Speaker = speaker;
				this.Speaker.transform.SetParent(this.SchematicObject.transform);
			}
			
			// Create TextToy and attach to the SchematicObject
			if (this.TextToyConfig.IsEnabled is true)
			{
				this.TextToy = TextToyManager.SpawnTextForSchematic(this.SchematicObject, this.TextToyConfig);
				if (this.TextToy is null)
				{
					Log.Error("Failed to create TextToy for custom role.");
				}
			}
			
			// Get Animator from SchematicObject
			this.Animator = SchematicManager.GetAnimatorFromSchematic(this.SchematicObject);
			if (this.Animator is null)
			{
				Log.Error("Failed to get Animator from custom role.");
			}

			if (this.IsPlayerInvisible is true)
			{
				this.SchematicObject.gameObject.AddComponent<MovementController>().
					Init(player, this.SchematicConfig.Offset);
			}
			else
			{
				this.SchematicObject.transform.SetParent(player.Transform);
			}

			this.CooldownController = player.GameObject.AddComponent<CooldownController>();
			
			// Attach HintController to the player
			if (this.HintConfig.IsEnabled is true)
			{
				Timing.CallDelayed(0.1f, () =>
				{
					this.HintController = player.GameObject.AddComponent<HintController>();
					this.HintController.Init(HintConfig);
				});
			}
		}

		private void DestroyObjects()
		{
			Object.Destroy(this.HintController);
			Object.Destroy(this.CooldownController);
			
			this.AudioPlayer.RemoveAllClips();
			this.AudioPlayer.Destroy();

			this.SchematicObject.Destroy();
		}
		
		private void OnRoundStarted()
		{
			if (this.SpawnConfig.MinPlayers <= 0 || this.SpawnConfig.SpawnChance <= 0)
				return;
        
			if (this.TrackedPlayers.Count >= this.SpawnProperties.Limit)
				return;
        
			for (int i = 0; i < this.SpawnProperties.Limit; i++)
			{
				float chance = (float) Exiled.Loader.Loader.Random.NextDouble() * 100f;
				Log.Debug($"chance {chance} and spawn chance {this.SpawnConfig.SpawnChance}");
				
				if (chance >= this.SpawnConfig.SpawnChance)
					continue;
				
				Player randomPlayer = Player.List.GetRandomValue(r => r.IsHuman && !r.IsNPC && r.CustomInfo == null);

				Timing.CallDelayed(0.05f, () =>
				{
					this.AddRole(randomPlayer);
				});
			}
		}

		private void OnPlayerValidatedVisibility(PlayerValidatedVisibilityEventArgs ev)
		{
			if (this.IsPlayerInvisible is not true)
				return;
			
			if (!this.Check(ev.Target))
				return;
			
			ev.IsVisible = false || ev.Target.CurrentSpectators.Contains(ev.Player);
		}

		// Unnecessary properties
		[YamlIgnore]
		public override float SpawnChance { get; set; }
	}
}
