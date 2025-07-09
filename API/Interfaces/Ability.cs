namespace RoleAPI.API.Interfaces
{
	using Controller;

	using Exiled.API.Features;

	using UnityEngine;

	using UserSettings.ServerSpecific;

	public abstract class Ability : IAbility
	{
		public virtual string Name { get; }
		public virtual string Description { get; }
		public virtual int KeyId { get; }
		public virtual KeyCode KeyCode { get; }
		public virtual float Cooldown { get; }

		public virtual void Register()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnKeybindActivateAbility;
		}

		public virtual void Unregister()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnKeybindActivateAbility;
		}

		private void OnKeybindActivateAbility(ReferenceHub referenceHub, ServerSpecificSettingBase settingBase)
		{
			// Check keybind settings
			if (settingBase is not SSKeybindSetting keybindSetting || keybindSetting.SettingId != KeyId || !keybindSetting.SyncIsPressed)
				return;

			// Check player and role
			if (!Player.TryGet(referenceHub, out Player player) ||
				ExtendedRole.Instances.TryGetValue(player, out ExtendedRole role))
			{
				return;
			}

			if (role.Animator != null)
			{
				// If the current animation is not idle, then in progress
				// I would like the animation of the ability to stop, as otherwise it will be possible to play multiple animations at a time
				AnimatorStateInfo stateInfo = role.Animator.GetCurrentAnimatorStateInfo(0);
				if (!stateInfo.IsName("IdleAnimation"))
				{
					return;
				}
			}

			// Check cooldown for an ability
			CooldownController cooldown = player.GameObject.GetComponent<CooldownController>();
			if (!cooldown.IsAbilityAvailable(Name))
				return;

			// Set a cooldown for an ability
			cooldown.SetCooldownForAbility(Name, Cooldown);

			// Activate an ability
			ActivateAbility(player, role);
			Log.Debug($"[Ability] Activating the {Name} ability");
		}

		protected abstract void ActivateAbility(Player player, ExtendedRole role);
	}
}