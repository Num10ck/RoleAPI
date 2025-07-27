namespace RoleAPI.API.Managers
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Exiled.API.Features;

	using Interfaces;

	public static class AbilityRegistrator
	{
		public static IAbility[] RegisterAbilities(Type[] abilityTypes)
		{
			List<IAbility> abilities = new();
			foreach (Type type in abilityTypes)
			{
				if (Activator.CreateInstance(type) is IAbility activator)
				{
					abilities.Add(activator);
					activator.Register();
					Log.Debug($"Register the {activator.Name} ability.");
				}
			}

			return abilities.ToArray();
		}
	}
}