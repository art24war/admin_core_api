using Microsoft.EntityFrameworkCore;

namespace DbModels
{
    public class UsersRoleRelation
    {
        public int Id { get; set; }
        public RoleModel Role { get; set; }
    }
}
