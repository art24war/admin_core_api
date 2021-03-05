using DbModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public UserTypeEnum UserType { get; set; }
        public List<UsersRoleRelation> Roles { get; set; }
        public string Comment { get; set; }
        public int? DealerId { get; set; }
        [ForeignKey("DealerId")]
        public DealerModel Dealer { get; set; }
        public int? CreatedByUser { get; set; }
        [ForeignKey("CreatedByUser")]
        public UserModel CreatedBy { get; set; }
        
    }
}
