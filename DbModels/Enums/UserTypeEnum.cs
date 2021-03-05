using System.ComponentModel.DataAnnotations;
using resourceLib;

namespace DbModels.Enums
{
    public enum UserTypeEnum
    {
        [Display(ResourceType = typeof(enums), Name = "admin_role" )]
        Admin = 1,
        [Display(ResourceType = typeof(enums), Name = "manager_role")]
        Manager = 2,

        Guest = 99
    }
}
