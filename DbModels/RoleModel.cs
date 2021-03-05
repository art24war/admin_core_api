
using Microsoft.EntityFrameworkCore;

namespace DbModels
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public RoleGroupModel RoleGroup { get; set; }
        public string Comment { get; set; }
    }
}
