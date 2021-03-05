using AutoMapper;
using DbModels;
using ReactReduxApi.Models;
using ReactReduxApi.Models.Users;
using System.Linq;
using System.Threading;

namespace ReactReduxApi.Helpers
{
    public class AutomapperProfile : Profile 
    {
        public AutomapperProfile()
        {
            CreateMap<UserModel, UserListModel>()
                .ForMember(m => m.Roles, mo => mo.MapFrom(o => o.Roles.Select(link => link.Role.RoleCode)))
                .ForMember(m => m.DealerName, mo => mo.MapFrom(o => o.Dealer != null ? o.Dealer.Name : "-"));
            CreateMap<RegisterUserModel, UserModel>()
                .ForMember(u => u.PasswordHash, mo => mo.MapFrom(o => CryptoHelper.GetSha256String(o.Password)))
                .ForMember(u=> u.Comment, mo=> mo.MapFrom(o=> o.Comment ?? "Creates from API"));
        }
    }
}
