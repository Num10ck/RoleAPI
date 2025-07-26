namespace RoleAPI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using CustomRoles;
	using CustomRoles.Interfaces;

	using Exiled.API.Features;

	public class SomeAbility : Ability
	{
		protected override void ActivateAbility(Player player, ExtendedRole role)
		{
			player.Broadcast(3, "hello");
		}
	}
}
