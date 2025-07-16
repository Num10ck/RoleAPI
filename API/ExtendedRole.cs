namespace RoleAPI.API
{
	using System.Collections.Generic;

	using Controller;

	using Exiled.API.Enums;
	using Exiled.API.Features;
	using Exiled.CustomRoles.API.Features;

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
		
		public abstract string SchematicName { get; set; }

		public abstract Vector3 SchematicOffset { get; set; }
		
		public abstract string TextToyColor { get; set; }
		
		public abstract int Volume { get; set; }
		
		public abstract bool IsPlayerInvisible { get; set; }
		
		public abstract List<EffectType> EffectList { get; set; }
		
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
			
			foreach (EffectType effectType in this.EffectList) //TODO add intensity
			{
				player.EnableEffect(effectType);
			}
			
			this.ShowMessage(player);
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
			this.SchematicObject = SchematicManager.AddSchematicByName(this.SchematicName);
			if (this.SchematicObject is null)
			{
				Log.Error("Failed to create Schematic for custom role.");
				return;
			}
			
			// Create AudioPlayer for player
			this.AudioPlayer = player.AddAudio(this.Volume);
			if (!this.AudioPlayer.TryGetSpeaker("scp999-speaker", out Speaker speaker)) //TODO
			{
				Log.Error("Failed to get Speaker from custom role.");
				return;
			}
			
			this.Speaker = speaker;
			
			// Create TextToy and attach to the SchematicObject
			this.TextToy = TextToyManager.CreateTextForSchematic(this.SchematicObject, this.Name, this.TextToyColor);
			if (this.TextToy is null)
			{
				Log.Error("Failed to create TextToy for custom role.");
			}
			
			// Get Animator from SchematicObject
			this.Animator = SchematicManager.GetAnimatorFromSchematic(this.SchematicObject);
			if (this.Animator is null)
			{
				Log.Error("Failed to get Animator from custom role.");
			}
			
			// Attach objects to the SchematicObject
			this.Speaker.transform.SetParent(this.SchematicObject.transform);

			if (this.IsPlayerInvisible is true)
			{
				this.SchematicObject.gameObject.AddComponent<MovementController>().
					Init(player, this.SchematicOffset);
			}
			else
			{
				this.SchematicObject.transform.SetParent(player.Transform);
			}
			
			// Attach HintController to the player
			Timing.CallDelayed(0.1f, () =>
			{
				this.HintController = player.GameObject.AddComponent<HintController>();
			});
		}

		private void DestroyObjects()
		{
			//Destroy(this.HintController);
			
			this.AudioPlayer.RemoveAllClips();
			this.AudioPlayer.Destroy();

			this.SchematicObject.Destroy();
		}
	}
}
