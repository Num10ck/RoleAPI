namespace RoleAPI.API.Configs
{
	using System;

	using Interfaces;

	public class AbilityConfig
	{
		public Type[] AbilityTypes { get; set; }

		public IAbility[] Abilities { get; set; }
	}
}