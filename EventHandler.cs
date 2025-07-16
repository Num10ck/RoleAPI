namespace RoleAPI
{
	using LabApi.Events.Arguments.PlayerEvents;

	using MEC;

	public class EventHandler
	{/*
		public EventHandler()
		{
			Exiled.Events.Handlers.Server.RoundStarted += this.OnRoundStarted;
			LabApi.Events.Handlers.PlayerEvents.ValidatedVisibility += this.OnPlayerValidatedVisibility;
		}
		
		/// <summary>
		/// Logic of choosing SCP-999 if the round is started
		/// </summary>
		private void OnRoundStarted()
		{
			_scp999role = CustomRole.Get(typeof(Scp999Role)) as Scp999Role;
			if (_scp999role is null)
			{
				Log.Error("Custom role SCP-999 role not found or not registered");
				return;
			}

			// Minimum and maximum number of Players for the chance of SCP-999 appearing
			float min = 0;
			float max = 1;

			if (min < 0 || max < 0)
			{
				Log.Error("Set the number of players to normal values in config");
				return;
			}
        
			// Add SCP-999 if no in the game
			if (_scp999role!.TrackedPlayers.Count >= _scp999role.SpawnProperties.Limit)
				return;
        
			for (int i = 0; i < _scp999role.SpawnProperties.Limit; i++)
			{
				// List of people who could potentially become SCP-999
				var players = Player.List.Where(r => r.IsHuman && !r.IsNPC && r.CustomInfo == null).ToList();
				// A minimum of players is required
				if (players.Count < min || players.Count == 0)
					return;
        
				// The formula for the chance of SCP-999 appearing in a round depends on count of players
				float value = Math.Max(min, Math.Min(max, Player.List.Count));
            
				// Choosing a random player
				Player randomPlayer = players.RandomItem();

				Timing.CallDelayed(0.05f, () =>
				{
					_scp999role!.AddRole(randomPlayer);
				});
			}
		}
		
		/// <summary>
		/// Making SCP-999 invisible to every player
		/// </summary>
		private void OnPlayerValidatedVisibility(PlayerValidatedVisibilityEventArgs ev)
		{
			if (_scp999role is null)
				return;

			if (_scp999role.Check(ev.Target))
			{
				ev.IsVisible = false;

				if (ev.Target.CurrentSpectators.Contains(ev.Player))
				{
					ev.IsVisible = true;
				}
			}
		}*/
	}
}