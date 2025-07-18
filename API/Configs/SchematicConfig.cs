namespace RoleAPI.API.Configs
{
	using UnityEngine;

	public class SchematicConfig
	{
		public string SchematicName { get; set; }
		public bool IsVisibleForOwner { get; set; } = true;
		public Vector3 Offset { get; set; } = Vector3.zero;
		public Vector3 Rotation { get; set; } = Vector3.zero;
		public Vector3 Scale { get; set; } = Vector3.one;
	}
}