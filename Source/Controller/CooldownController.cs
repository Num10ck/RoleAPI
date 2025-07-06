namespace CustomRoles.Features.Controller
{
	using System.Collections.Generic;
	using System.Linq;

	using CustomRoles.Features.Managers;

	using Exiled.API.Features;

	using UnityEngine;

	public class CooldownController : MonoBehaviour
	{
		/// <summary>
		/// Register features for the player
		/// </summary>
		void Awake()
		{
			this._abilityCooldown = AbilityRegistrator.GetAbilities.ToDictionary(a => a.Name, _ => 0f);
			InvokeRepeating(nameof(CheckCooldown), 0f, 1f);
			Log.Debug($"[CooldownController] Invoke the cooldown cycle");
		}

		/// <summary>
		/// Cycle that counts every second of an ability's cooldown
		/// </summary>
		void CheckCooldown()
		{
			foreach (var key in this._abilityCooldown.Keys.ToList())
			{
				if (this._abilityCooldown[key] > 0)
				{
					this._abilityCooldown[key]--;
				}
				else
				{
					this._abilityCooldown[key] = 0;
				}
			}
		}

		/// <summary>
		/// Unregister features for the player
		/// </summary>
		void OnDestroy()
		{
			CancelInvoke(nameof(CheckCooldown));
			Log.Debug($"[CooldownController] Cancel the cooldown cycle");
		}

		// Properties
		public bool IsAbilityAvailable(string ability) => this._abilityCooldown[ability] <= 0;
		public void SetCooldownForAbility(string ability, float time) => this._abilityCooldown[ability] = time;

		// Fields
		private Dictionary<string, float> _abilityCooldown;
	}
}