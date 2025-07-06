namespace CustomRoles.Features.Managers
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Exiled.API.Features;

	using CustomRoles.Interfaces;

	public static class AbilityRegistrator
	{
		private readonly static List<IAbility> _abilityList = [];
		public static void RegisterAbilities()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsInterface || type.IsAbstract || !type.GetInterfaces().Contains(typeof(IAbility)))
					continue;

				IAbility activator = Activator.CreateInstance(type) as IAbility;
				if (activator != null)
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

		public static List<IAbility> GetAbilities => _abilityList;
	}
}