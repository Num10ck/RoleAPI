namespace RoleAPI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using CustomRoles.Features.Managers;

	using Exiled.API.Enums;
	using Exiled.API.Extensions;
	using Exiled.API.Features;
	using Exiled.API.Interfaces;
	using Exiled.CustomRoles.API.Features;

	using LabApi
using LabApi.Events.Arguments.PlayerEvents;

	public class SomePlugin : Plugin<Config>
	{
		public override void OnEnabled()
		{
			Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
		}

		private void OnRoundStarted()
		{
			Player ply = Player.List.GetRandomValue();

			ply.EnableEffect(EffectType.Ensnared);

			SomeRole role = CustomRole.Get(1984) as SomeRole;

			role.Create(
				ply,
				SchematicManager.AddSchematicByName("aboba"),
				null,
				100
			);
		}
	}

	public class Config : IConfig
	{
		public bool IsEnabled { get; set; }
		public bool Debug { get; set; }
	}
}
