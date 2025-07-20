namespace RoleAPI.API.Controller
{
	using System.Collections.Generic;
	using System.Linq;

	using Exiled.API.Features;

	using Managers;

	using UnityEngine;

	public class CooldownController : MonoBehaviour
	{
		private Dictionary<string, float> _abilityCooldown;
		
		void Awake()
		{
			_abilityCooldown = AbilityRegistrator.GetAbilities.ToDictionary(a => a.Name, _ => 0f);
			InvokeRepeating(nameof(CheckCooldown), 0f, 1f);
			Log.Debug($"[CooldownController] Invoke the cooldown cycle");
		}

		void CheckCooldown()
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
			CancelInvoke(nameof(CheckCooldown));
			Log.Debug($"[CooldownController] Cancel the cooldown cycle");
		}

		public bool IsAbilityAvailable(string ability) => _abilityCooldown[ability] <= 0;
		public void SetCooldownForAbility(string ability, float time) => _abilityCooldown[ability] = time;
	}
}