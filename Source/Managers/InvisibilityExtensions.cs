namespace CustomRoles.Features.Managers
{
	using Exiled.API.Features;
	using Exiled.API.Features.Roles;

	public static class InvisibilityExtensions
	{
		/// <summary>
		/// Make a specific SCP-999 invisible to all players
		/// </summary>
		/// <param name="player">A player with the role of SCP-999</param>
		public static void MakeInvisible(this Player player)
		{
			foreach (Player other in Player.List)
			{
				if (player == other)
					continue;

				if (player.Role.Is(out FpcRole fpc))
				{
					fpc.IsInvisibleFor.Add(other);
				}
			}
		}

		/// <summary>
		/// Make a specific SCP-999 invisible for a specific player
		/// </summary>
		/// <param name="scp999">A player with the role of SCP-999</param>
		/// <param name="player">The player who shouldn't see SCP-999</param>
		public static void MakeInvisibleForPlayer(this Player scp999, Player player)
		{
			if (scp999.Role.Is(out FpcRole fpc))
			{
				fpc.IsInvisibleFor.Add(player);
			}
		}

		/// <summary>
		/// Remove the invisibility of a specific SCP-999 for all players
		/// </summary>
		/// <param name="scp999">A player with the role of SCP-999</param>
		public static void RemoveInvisibility(this Player scp999)
		{
			if (scp999.Role.Is(out FpcRole fpc))
			{
				foreach (Player player in fpc.IsInvisibleFor)
				{
					fpc.IsInvisibleFor.Remove(player);
				}
			}
		}

		/// <summary>
		/// Remove the invisibility of a specific SCP-999 for a specific player
		/// </summary>
		/// <param name="scp999">A player with the role of SCP-999</param>
		/// <param name="player">The player who should see SCP-999</param>
		public static void RemoveInvisibilityForPlayer(this Player scp999, Player player)
		{
			if (scp999.Role.Is(out FpcRole fpc))
			{
				fpc.IsInvisibleFor.Remove(player);
			}
		}
	}
}