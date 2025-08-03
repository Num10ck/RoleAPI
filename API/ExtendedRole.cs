namespace RoleAPI.API
{
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	using Configs;

	using Exiled.API.Enums;
	using Exiled.API.Extensions;
	using Exiled.API.Features;
	using Exiled.CustomRoles.API.Features;
	using Exiled.Events.EventArgs.Player;

	using LabApi.Events.Arguments.PlayerEvents;

	using Managers;

	using MEC;

	using PlayerRoles;

	using YamlDotNet.Serialization;

	using Cassie = Exiled.API.Features.Cassie;
	using Player = Exiled.API.Features.Player;

	public abstract class ExtendedRole : CustomRole
	{
		public static IReadOnlyDictionary<Player, ObjectManager> Instances
		{
			get => _instances;
		}
		
		private static readonly Dictionary<Player, ObjectManager> _instances = [];
		
		public abstract string CustomDeathText { get; set; }

		public abstract string CassieDeathAnnouncement { get; set; }
		
		public abstract SpawnConfig SpawnConfig { get; set; }
		
		public abstract SchematicConfig SchematicConfig { get; set; }
		
		public abstract TextToyConfig TextToyConfig { get; set; }
		
		public abstract HintConfig HintConfig { get; set; }
		
		public abstract AudioConfig AudioConfig { get; set; }

		[YamlIgnore]
		public abstract AbilityConfig AbilityConfig { get; set; }
		
		public abstract List<EffectConfig> Effects { get; set; }
		
		public abstract bool IsPlayerInvisible { get; set; }
		
		public abstract bool IsShowPlayerNickname { get; set; }
		
		protected override void SubscribeEvents()
		{
			// Register abilities and keybinds
			this.AbilityConfig.Abilities = AbilityRegistrator.RegisterAbilities(AbilityConfig.AbilityTypes);
			KeybindManager.RegisterKeybinds(this.Name, this.AbilityConfig.Abilities);
			
			// Subscribe events
			base.SubscribeEvents();
			Exiled.Events.Handlers.Player.Dying += this.OnDying;
			Exiled.Events.Handlers.Server.RoundStarted += this.OnRoundStarted;
			LabApi.Events.Handlers.PlayerEvents.ValidatedVisibility += this.OnPlayerValidatedVisibility;
		}

		protected override void UnsubscribeEvents()
		{
			Exiled.Events.Handlers.Player.Dying -= this.OnDying;
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

			ObjectManager manager = new();
			manager.CreateObjects(player, this);
			
			player.SessionVariables["risottoMan.customRoles"] = this.Name;
			_instances.Add(player, manager);
		}

		public override void RemoveRole(Player player)
		{
			if (_instances.TryGetValue(player, out ObjectManager manager))
			{
				manager.DestroyObjects();
				_instances.Remove(player);
			}
			
			player.SessionVariables.Remove("risottoMan.customRoles");
			player.CustomName = null;
			
			base.RemoveRole(player);
		}

		private void OnDying(DyingEventArgs ev)
		{
			if (!this.Check(ev.Player))
				return;
 
			string numbers = Regex.Replace(this.Name, "[^0-9]+", string.Empty); // SCP-999 -> 999
			numbers = string.Join(" ", numbers.ToCharArray()); // 999 -> 9 9 9
			Cassie.MessageTranslated($"SCP {numbers} contained successfully.", this.CassieDeathAnnouncement);
		}
		
		private void OnRoundStarted()
		{
			if (this.SpawnConfig.MinPlayers > Player.List.Count || this.SpawnConfig.SpawnChance <= 0)
				return;
			
			if (this.TrackedPlayers.Count >= this.SpawnProperties.Limit)
				return;
			
			for (int i = 0; i < this.SpawnProperties.Limit; i++)
			{
				float chance = (float) Exiled.Loader.Loader.Random.NextDouble() * 100f;
				Log.Debug($"chance {chance} and spawn chance {this.SpawnConfig.SpawnChance}");
				
				if (chance >= this.SpawnConfig.SpawnChance)
					continue;

				Player randomPlayer = Player.List.GetRandomValue(r =>
					r.IsHuman && (!r.IsNPC || this.SpawnConfig.IsSpawnForDummy) && r.CustomInfo == null);

				if (randomPlayer.SessionVariables.ContainsKey("risottoMan.customRoles"))
					return;
				
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
