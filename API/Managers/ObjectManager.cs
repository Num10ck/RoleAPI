namespace RoleAPI.API.Managers
{
	using Exiled.API.Features;

	using LabApi.Features.Wrappers;

	using MEC;

	using ProjectMER.Features.Objects;

	using Controller;

	using UnityEngine;

	using Player = Exiled.API.Features.Player;

	public class ObjectManager
	{
		public SchematicObject SchematicObject { get; set; }
		public AudioPlayer AudioPlayer { get; set; }
		public Animator Animator { get; set; }
		public Speaker Speaker { get; set; }
		public TextToy TextToy { get; set; }
		public HintController HintController { get; set; }
		public CooldownController CooldownController { get; set; }
		
		public void CreateObjects(Player player, ExtendedRole config)
		{
			// Create SchematicObject from SchematicName
			this.SchematicObject = SchematicManager.SpawnSchematic(config.SchematicConfig);
			if (this.SchematicObject is null)
			{
				Log.Debug("Failed to create Schematic for custom role.");
				return;
			}
			
			// Make schematic invisible for owner
			if (config.SchematicConfig.IsSchematicVisibleForOwner is false)
			{
				SchematicManager.MakeSchematicInvisibleForOwner(this.SchematicObject, player);
			}
			
			// Create AudioPlayer for player
			if (config.AudioConfig.Volume > 0)
			{
				this.AudioPlayer = player.AddAudio(config.AudioConfig);
				if (!this.AudioPlayer.TryGetSpeaker($"{config.AudioConfig.Name}-speaker", out Speaker speaker))
				{
					Log.Debug("Failed to get Speaker from custom role.");
					return;
				}
				
				// Attach speaker to schematic
				this.Speaker = speaker;
				this.Speaker.transform.SetParent(this.SchematicObject.transform);
			}
			
			// Create TextToy and attach to the SchematicObject
			if (config.TextToyConfig.IsEnabled is true)
			{
				this.TextToy = TextToyManager.SpawnTextForSchematic(this.SchematicObject, config.TextToyConfig);
				if (this.TextToy is not null)
				{
					// Make TextToy invisible for Player
					TextToyManager.MakeTextInvisibleForOwner(this.TextToy, player);
				}
				else
				{
					Log.Debug("Failed to create TextToy for custom role.");
				}
			}
			
			// Get Animator from SchematicObject
			
			this.Animator = SchematicManager.GetAnimatorFromSchematic(this.SchematicObject);
			if (this.Animator is null)
			{
				Log.Debug("Failed to get Animator from custom role.");
			}
			
			if (config.IsPlayerInvisible is true)
			{
				this.SchematicObject.gameObject.AddComponent<MovementController>().
					Init(player, config.SchematicConfig.Offset);
			}
			else
			{
				this.SchematicObject.transform.position = player.Position + config.SchematicConfig.Offset;
				
				if (config.SchematicConfig.IsAttachToCamera)
				{
					this.SchematicObject.transform.parent = player.CameraTransform; //todo
				}
				else
				{
					this.SchematicObject.transform.parent = player.Transform;
				}
			}
			
			this.CooldownController = player.GameObject.AddComponent<CooldownController>();
			
			// Attach HintController to the player
			if (config.HintConfig.IsEnabled is true)
			{
				Timing.CallDelayed(0.1f, () =>
				{
					this.HintController = player.GameObject.AddComponent<HintController>();
					this.HintController.Init(config.HintConfig);
				});
			}
		}

		public void DestroyObjects()
		{
			Object.Destroy(this.HintController);
			Object.Destroy(this.CooldownController);
			
			this.AudioPlayer.RemoveAllClips();
			this.AudioPlayer.Destroy();

			this.SchematicObject.Destroy();
		}
	}
}