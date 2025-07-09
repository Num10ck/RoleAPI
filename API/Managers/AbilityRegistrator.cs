namespace RoleAPI.API.Managers
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Exiled.API.Features;

	using Interfaces;

	public static class AbilityRegistrator
	{
		public static IReadOnlyList<IAbility> GetAbilities => _abilityList;

		private static readonly List<IAbility> _abilityList = [];
		public static void RegisterAbilities()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsInterface || type.IsAbstract || !type.GetInterfaces().Contains(typeof(IAbility)))
					continue;

				if (Activator.CreateInstance(type) is IAbility activator)
				{
					_abilityList.Add(activator);

					Log.Debug($"Register the {activator.Name} ability.");

					activator.Register();
				}
			}
		}

		public static void UnregisterAbilities()
		{
			foreach (IAbility ability in _abilityList)
			{
				ability.Unregister();
			}
		}
	}
}