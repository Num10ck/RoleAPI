namespace RoleAPI.API.Patches
{
	using System.Diagnostics;

	using Exiled.API.Features;

	using HarmonyLib;

	[HarmonyPatch(typeof(ProjectMER.ProjectMER), nameof(ProjectMER.ProjectMER.SchematicsDir), MethodType.Getter)]
	internal class SchematicMerPatch
	{
		public static bool Prefix(ref string __result)
		{
			var stackTrace = new StackTrace();
			foreach (var frame in stackTrace.GetFrames())
			{
				var declaringType = frame.GetMethod().DeclaringType;
				var assemblyName = declaringType.Assembly.GetName().Name;

				if (assemblyName == "RoleAPI" && declaringType.Name == "SchematicManager")
				{
					Log.Debug($"Get schematic from {Startup.SchematicPath}");
					__result = Startup.SchematicPath;
					return false;
				}
			}

			return true;
		}
	}
}