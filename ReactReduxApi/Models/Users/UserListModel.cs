using DbModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactReduxApi.Models.Users
{
    public class UserListModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public UserTypeEnum UserType { get; set; }
        public List<string> Roles { get; set; }
        public string Comment { get; set; }
        public string DealerName { get; set; }
    }
}
