namespace RoleAPI.API.Controller
{
	using System.Collections.Generic;
	using System.Linq;

	using Configs;

	using Exiled.API.Features;

	using UnityEngine;

	public class CooldownController : MonoBehaviour
	{
		private Dictionary<string, float> _abilityCooldown;
		
		public void Init(AbilityConfig abilityConfig)
		{
			_abilityCooldown = abilityConfig.Abilities.ToDictionary(a => a.Name, _ => 0f);
			InvokeRepeating(nameof(UpdateCooldown), 0f, 1f);
			Log.Debug($"[CooldownController] Invoke the cooldown cycle");
		}

		void UpdateCooldown()
		{
			foreach (var key in _abilityCooldown.Keys.ToList())
			{
				if (_abilityCooldown[key] > 0)
				{
					_abilityCooldown[key]--;
				}
				else
				{
					_abilityCooldown[key] = 0;
				}
			}
		}

		void OnDestroy()
		{
			CancelInvoke(nameof(UpdateCooldown));
			Log.Debug($"[CooldownController] Cancel the cooldown cycle");
		}

		public bool IsAbilityAvailable(string ability) => _abilityCooldown[ability] <= 0;
		public void SetCooldownForAbility(string ability, float time) => _abilityCooldown[ability] = time;
	}
}