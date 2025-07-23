namespace RoleAPI.API.Configs
{
	public class SpawnConfig
	{
		public bool IsSpawnForDummy { get; set; } = false;
		public int MinPlayers { get; set; } = 5;
		public float SpawnChance { get; set; } = 100;
	}
}