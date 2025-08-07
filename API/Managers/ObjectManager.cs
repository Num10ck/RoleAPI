namespace RoleAPI.API.Managers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Controller;

	using Exiled.API.Features;

	using Interfaces;

	using LabApi.Features.Wrappers;

	using MEC;

	using ProjectMER.Features.Objects;

	using UnityEngine;

	using Player = Exiled.API.Features.Player;

	public sealed class ObjectManager
	{
		public SchematicObject SchematicObject { get; set; }

		public AudioPlayer AudioPlayer { get; set; }

		public Animator Animator { get; set; }

		public Speaker Speaker { get; set; }

		public TextToy TextToy { get; set; }

		public HintController HintController { get; set; }

		public CooldownController CooldownController { get; set; }

		public Type[] AllowedAbilities { get; set; }
		
		public void CreateObjects(Player player, ExtendedRole config)
		{
			// Create SchematicObject from SchematicName
			SchematicObject = SchematicManager.SpawnSchematic(config.SchematicConfig);
			if (SchematicObject is null)
			{
				Log.Debug("Failed to create Schematic for custom role.");
				return;
			}
			
			// Make schematic invisible for owner
			if (config.SchematicConfig.IsSchematicVisibleForOwner is false)
			{
				SchematicManager.MakeSchematicInvisibleForOwner(SchematicObject, player);
			}
			
			// Create AudioPlayer for player
			if (config.AudioConfig.Volume > 0)
			{
				AudioPlayer = player.AddAudio(config.AudioConfig);
				if (!AudioPlayer.TryGetSpeaker($"{config.AudioConfig.Name}-speaker", out Speaker speaker))
				{
					Log.Debug("Failed to get Speaker from custom role.");
					return;
				}
				
				// Attach speaker to schematic
				Speaker = speaker;
				Speaker.transform.SetParent(SchematicObject.transform);
			}
			
			// Create TextToy and attach to the SchematicObject
			if (config.TextToyConfig.IsEnabled is true)
			{
				TextToy = TextToyManager.SpawnTextForSchematic(SchematicObject, config.TextToyConfig);
				if (TextToy is not null)
				{
					// Make TextToy invisible for Player
					TextToyManager.MakeTextInvisibleForOwner(TextToy, player);
				}
				else
				{
					Log.Debug("Failed to create TextToy for custom role.");
				}
			}
			
			// Get Animator from SchematicObject
			Animator = SchematicManager.GetAnimatorFromSchematic(SchematicObject);
			if (Animator is null)
			{
				Log.Debug("Failed to get Animator from custom role.");
			}
			
			// Attach schematic to the player
			SchematicObject.transform.position = player.Position + config.SchematicConfig.Offset;
			SchematicObject.transform.rotation = player.Rotation;
			SchematicObject.transform.parent = player.Transform;
			
			/* todo
			// Attach schematic to the player camera
			if (config.SchematicConfig.IsAttachToCamera)
			{
				var hitboxes = player.GameObject.GetComponentsInChildren<HitboxIdentity>();
				foreach (var hitbox in hitboxes)
				{
					SchematicObject.transform.parent = hitbox.transform;
				}
			}*/
			
			// Attach CooldownController to the player
			CooldownController = player.GameObject.AddComponent<CooldownController>();
			CooldownController.Init(config.AbilityConfig);
			
			// Attach HintController to the player
			if (config.HintConfig.IsEnabled is true)
			{
				Timing.CallDelayed(0.1f, () =>
				{
					HintController = player.GameObject.AddComponent<HintController>();
					HintController.Init(config.HintConfig, config.AbilityConfig);
				});
			}

			AllowedAbilities = config.AbilityConfig.AbilityTypes;
		}

		public void DestroyObjects()
		{
			UnityEngine.Object.Destroy(HintController);
			UnityEngine.Object.Destroy(CooldownController);
			
			AudioPlayer.RemoveAllClips();
			AudioPlayer.Destroy();

			SchematicObject.Destroy();
		}
	}
}