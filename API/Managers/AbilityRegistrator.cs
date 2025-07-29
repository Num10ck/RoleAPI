namespace RoleAPI.API.Managers
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Exiled.API.Features;

	using Interfaces;

	public static class AbilityRegistrator
	{
		public static IAbility[] RegisterAbilities(string[] abilityTypes)
		{
			List<IAbility> abilities = new();

			foreach (string name in abilityTypes)
			{
				Type type = Type.GetType(name);

				if (type == null)
				{
					Log.Error($"Type {name} not found.");
					continue;
				}

				if (Activator.CreateInstance(type) is IAbility activator)
				{
					abilities.Add(activator);
					activator.Register();
					Log.Debug($"Register the {activator.Name} ability.");
				}
				else
				{
					Log.Debug($"Provided type {name} is not IAbility.");
				}
			}

			return [.. abilities];
		}
	}
}