using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbModels
{
    public class RefreshToken
    {
        [ForeignKey("User")]
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ValidTo { get; set; }
        public virtual UserModel User { get; set; }
    }
}