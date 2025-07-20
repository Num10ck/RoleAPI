namespace RoleAPI.API.Configs
{
	public class HintConfig
	{
		public bool IsEnabled { get; set; } = true;
		public bool IsHintServiceMeowEnabled { get; set; } = false;
		public string Text { get; set; }
		public string AvailableAbilityColor { get; set; }
		public string UnavailableAbilityColor { get; set; }
	}
}