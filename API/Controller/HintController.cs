namespace RoleAPI.API.Controller
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using Configs;

	using Exiled.API.Features;

	using Hints;

	using Interfaces;

	using UnityEngine;

	public class HintController : MonoBehaviour
	{
		private Player _player;
		private List<IAbility> _abilities;
		private CooldownController _controller;
		private HintConfig _config;
		
		public void Init(HintConfig config)
		{
			_player = Player.Get(gameObject);
			_controller = gameObject.GetComponent<CooldownController>();
			_abilities = Managers.AbilityRegistrator.GetAbilities.OrderBy(r => r.KeyId).ToList();
			_config = config;
			
			InvokeRepeating(nameof(UpdateHint), 0f, 0.5f);
			Log.Debug($"[CooldownController] Invoke the hint cycle");
		}
    
		void UpdateHint()
		{
			List<HintParameter> parameters = [];
			StringBuilder text = new StringBuilder(_config.Text);
			
			foreach (var ability in _abilities)
			{
				string color = _controller.IsAbilityAvailable(ability.Name) 
					? _config.AvailableAbilityColor 
					: _config.UnavailableAbilityColor;

				int index = text.ToString().IndexOf("%color%", StringComparison.Ordinal);
				if (index != -1)
				{
					text.Remove(index, "%color%".Length);
					text.Insert(index, color);
				}
				
				parameters.Add(new SSKeybindHintParameter(ability.KeyId));
			}
			
			text.Append("\n\n\n\n\n\n\n");
			_player.HintDisplay.Show(new TextHint(
				text.ToString(), 
				parameters.ToArray(), 
				durationScalar: 1f));
		}
    
		void OnDestroy()
		{
			CancelInvoke(nameof(UpdateHint));
			Log.Debug($"[CooldownController] Cancel the hint cycle");
		}
	}
}