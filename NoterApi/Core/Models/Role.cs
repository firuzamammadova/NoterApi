using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Role
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }

    public class RoleUI
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }

    public class RoleGroupby
    {
        public long GroupId { get; set; }
        public string GroupName { get; set; }
        public List<RoleUI> Roles { get; set; }
    }
}
