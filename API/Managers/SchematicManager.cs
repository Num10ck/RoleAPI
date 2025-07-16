namespace RoleAPI.API.Managers
{
	using ProjectMER.Features;
	using ProjectMER.Features.Objects;

	using UnityEngine;

	public static class SchematicManager
	{
		public static SchematicObject AddSchematicByName(string schematicName)
		{
			return ObjectSpawner.SpawnSchematic(
				schematicName,
				Vector3.zero,
				Vector3.zero,
				Vector3.one
			);
		}

		public static Animator GetAnimatorFromSchematic(SchematicObject schematicObject)
		{
			return schematicObject?.GetComponentInChildren<Animator>(true);
		}
	}
}