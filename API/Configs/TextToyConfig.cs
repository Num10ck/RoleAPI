namespace RoleAPI.API.Configs
{
	using UnityEngine;

	public class TextToyConfig
	{
		public bool IsEnabled { get; set; } = true;
		public string Text { get; set; }
		public Vector3 Offset { get; set; } = Vector3.zero;
		public Vector3 Rotation { get; set; } = Vector3.zero;
		public Vector3 Scale { get; set; } = Vector3.one;
	}
}