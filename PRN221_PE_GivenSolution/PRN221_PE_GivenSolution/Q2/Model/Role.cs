using System;
using System.Collections.Generic;

namespace Q2.Model
{
    public partial class Role
    {
        public Role()
        {
            Members = new HashSet<Member>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Member> Members { get; set; }
    }
}
