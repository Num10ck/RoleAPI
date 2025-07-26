namespace RoleAPI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using CustomRoles;

	public class SomeRole : ExtendedRole
	{
		public override uint Id { get; set; } = 1984;
		public override int MaxHealth { get; set; } = 100;
		public override string Name { get; set; } = "SCP-035";
		public override string Description { get; set; } = "This SCP is very good!";
		public override string CustomInfo { get; set; } = "SCP-035";
	}
}
