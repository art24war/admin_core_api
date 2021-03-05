using System.Collections.Generic;

namespace DbModels
{
    public class RoleGroupModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public List<RoleModel> UserRoles { get; set; }
    }
}