namespace RoleAPI.API.Managers
{
	using Configs;

	using LabApi.Features.Wrappers;

	using ProjectMER.Features.Objects;

	using UnityEngine;

	public static class TextToyManager
	{
		public static TextToy SpawnTextForSchematic(
			SchematicObject parent, 
			TextToyConfig config)
		{
			TextToy textToyObject = TextToy.Create(networkSpawn: false);
				
			textToyObject.TextFormat = config.Text;
			
			textToyObject.Parent = parent.transform;
			textToyObject.Transform.localPosition += config.Offset;
			textToyObject.Transform.localRotation = Quaternion.Euler(config.Rotation);
			textToyObject.Transform.localScale = config.Scale;
			
			textToyObject.Spawn();

			return textToyObject;
		}
	}
}