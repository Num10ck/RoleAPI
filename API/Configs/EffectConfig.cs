namespace RoleAPI.API.Configs
{
	using Exiled.API.Enums;

	public class EffectConfig
	{
		public EffectType EffectType { get; set; }
		public byte Intensity { get; set; } = 0;
	}
}