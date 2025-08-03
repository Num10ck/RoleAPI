namespace RoleAPI.API.Configs
{
	using System;

	using Interfaces;

	using YamlDotNet.Serialization;

	public class AbilityConfig
	{
		public string[] AbilityTypes { get; set; }

		[YamlIgnore]
		public IAbility[] Abilities { get; set; }
	}
}