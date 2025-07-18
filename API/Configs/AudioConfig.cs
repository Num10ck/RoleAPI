namespace RoleAPI.API.Configs
{
	public class AudioConfig
	{
		public string Name { get; set; }
		public int Volume { get; set; } = 100;
		public bool IsSpatial { get; set; } = true;
		public float MinDistance { get; set; } = 5f;
		public float MaxDistance { get; set; } = 15f;
	}
}