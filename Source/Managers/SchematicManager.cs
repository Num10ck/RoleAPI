namespace CustomRoles.Features.Managers
{
	using Exiled.API.Features;

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
			Animator animator = schematicObject?.GetComponentInChildren<Animator>(true);
			if (animator == null)
			{
				Log.Error("The animator was not found");
			}

			return animator;
		}
	}
}