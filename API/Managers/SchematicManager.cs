namespace RoleAPI.API.Managers
{
	using Configs;

	using Exiled.API.Features;

	using Mirror;

	using ProjectMER.Features;
	using ProjectMER.Features.Objects;

	using UnityEngine;

	public static class SchematicManager
	{
		public static SchematicObject SpawnSchematic(SchematicConfig config)
		{
			return ObjectSpawner.SpawnSchematic(
				config.SchematicName,
				Vector3.zero,
				config.Rotation,
				config.Scale
			);
		}

		public static Animator GetAnimatorFromSchematic(SchematicObject schematicObject)
		{
			return schematicObject?.GetComponentInChildren<Animator>(true);
		}

		public static void MakeSchematicInvisibleForOwner(SchematicObject schematicObject, Player player)
		{
			foreach (var identity in schematicObject.NetworkIdentities)
			{
				player.Connection.Send(new ObjectDestroyMessage
				{
					netId = identity.netId
				});
			}
		}
	}
}