#define DEBUG

#if DEBUG
namespace RoleAPI.API.Patches
{
	using System.Diagnostics;

	using HarmonyLib;

	[HarmonyPatch(typeof(ProjectMER.ProjectMER), nameof(ProjectMER.ProjectMER.SchematicsDir), MethodType.Getter)]
	internal class SchematicMerPatch
	{
		public const string SCHEMATIC_PATH = "Schematics";

		public static bool Prefix(ref string __result)
		{
			var stackTrace = new StackTrace();
			foreach (var frame in stackTrace.GetFrames())
			{
				var declaringType = frame.GetMethod().DeclaringType;
				var assemblyName = declaringType.Assembly.GetName().Name;

				if (assemblyName == "Scp999" && declaringType.Name == "SchematicManager")
				{
					__result = SCHEMATIC_PATH;
					return false;
				}
			}

			return true;
		}
	}
}

#endif