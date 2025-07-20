namespace RoleAPI.API.Controller
{
	using System.Collections.Generic;
	using System.Linq;

	using Configs;

	using Exiled.API.Features;

	using Interfaces;

	using UnityEngine;

	public class HintController : MonoBehaviour
	{
		private Player _player;
		private List<IAbility> _abilities;
		private CooldownController _controller;
		private string _text;
		
		public void Init(HintConfig config)
		{
			_player = Player.Get(gameObject);
			_controller = gameObject.GetComponent<CooldownController>();
			_abilities = Managers.AbilityRegistrator.GetAbilities.OrderBy(r => r.KeyId).ToList();
			_text = config.Text;
			
			InvokeRepeating(nameof(CheckHint), 0f, 0.5f);
			Log.Debug($"[CooldownController] Invoke the hint cycle");
		}
    
		void CheckHint()
		{
			/* TODO How? 
			foreach (var ability in _abilities)
			{
				string color = "#ffa500";
				if (!_controller.IsAbilityAvailable(ability.Name))
				{
					color = "#966100";
				}
                    
				stringBuilder.Append($"<color={color}>{ability.Name}  [{ability.KeyCode}]</color>\n");
			}*/
			
			_player.ShowHint(_text, 1f);
		}
    
		void OnDestroy()
		{
			CancelInvoke(nameof(CheckHint));
			Log.Debug($"[CooldownController] Cancel the hint cycle");
		}
	}
}