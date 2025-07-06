namespace CustomRoles.Features.Managers
{
	using System;
	using System.Linq;
	using System.Text;

	using Exiled.API.Features;

	using HintServiceMeow.Core.Enum;
	using HintServiceMeow.Core.Utilities;

	using CustomRoles.Features.Controller;
	using CustomRoles.Interfaces;

	using Hint = HintServiceMeow.Core.Models.Hints.Hint;

	public static class HintExtensions
	{
		public static void AddHint(this Player player)
		{
			IOrderedEnumerable<IAbility> abilityList = AbilityRegistrator.GetAbilities.OrderBy(r => r.KeyId);

			Hint hint = new()
			{
				Id = "999",
				AutoText = arg =>
				{
					var controller = player.GameObject.GetComponent<CooldownController>();
					StringBuilder stringBuilder = new StringBuilder();

					stringBuilder.Append("<size=50><color=#ffa500>\ud83d\ude06 <b>SCP-999</b></color></size>\n");
					stringBuilder.Append("Abilities:\n");

					foreach (IAbility ability in abilityList)
					{
						string color = "#ffa500";
						if (!controller.IsAbilityAvailable(ability.Name))
						{
							color = "#966100";
						}

						stringBuilder.Append($"<color={color}>{ability.Name}  [{ability.KeyCode}]</color>\n");
					}

					stringBuilder.Append($"\n<size=18>if you cant use abilities\n" +
										 $"remove \u2b50 in settings</size>");

					return stringBuilder.ToString();
				},
				FontSize = 35,
				YCoordinate = 500,
				Alignment = HintAlignment.Right,
				SyncSpeed = HintSyncSpeed.Normal,
			};

			PlayerDisplay playerDisplay = PlayerDisplay.Get(player);
			playerDisplay.AddHint(hint);
		}

		public static void RemoveHint(Player player)
		{
			PlayerDisplay playerDisplay = PlayerDisplay.Get(player);
			playerDisplay.RemoveHint("999");
		}
	}
}