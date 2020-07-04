using AutoMapper;
using MadPay724.Common.Helpers;
using MadPay724.Common.Helpers.Helpers;
using MadPay724.Common.Helpers.MediaTypes;
using MadPay724.Data.Dtos.Common;
using MadPay724.Data.Dtos.Common.ION;
using MadPay724.Data.Dtos.Site.Admin.BankCard;
using MadPay724.Data.Dtos.Site.Admin.Photos;
using MadPay724.Data.Dtos.Site.Admin.Users;
using MadPay724.Data.Models;
using System.Linq;

namespace MadPay724.Presentation.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest=>dest.PhotoUrl , opt=>
                    {
                        opt.MapFrom(src => src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                    })
                 .ForMember(dest => dest.Age, opt =>
                     {
                         opt.MapFrom(src => src.BirthDate.ToAge());
                     });
            #region CreateMap<User,UserForListDto>();
            //CreateMap<User, UserForListDto>();

            CreateMap<User, UserForListDto>()
               .ForMember(dest => dest.Self, opt =>
                   opt.MapFrom(src =>
                   Link.To(nameof(Controllers.Site.V1.Admin.UsersController.GetUser), new { id = src.Id })));

            //CreateMap<User, UserForListDto>()
            //    .ForMember(dest => dest.Href, opt =>
            //        opt.MapFrom(src =>
            //        Link.To(nameof(Controllers.Site.V1.Admin.UsersController.GetUser), new { id = src.Id })))
            //     .ForMember(dest => dest.UpdateUser, opt =>
            //        opt.MapFrom(src =>
            //        Link.To(nameof(Controllers.Site.V1.Admin.UsersController.UpdateUser), new
            //        {
            //            id = src.Id,
            //            userForUpdateDto = new UserForUpdateDto()
            //        })));

            //CreateMap<User, UserForListDto>()
            //    .ForMember(dest => dest.Self, opt =>
            //        opt.MapFrom(src =>
            //        Link.To(nameof(Controllers.Site.V1.Admin.UsersController.GetUser), new { id = src.Id })))
            //     .ForMember(dest => dest.UpdateUser, opt =>
            //        opt.MapFrom(src =>
            //        Link.To(nameof(Controllers.Site.V1.Admin.UsersController.UpdateUser), new
            //        {
            //            id = src.Id,
            //            userForUpdateDto = new UserForUpdateDto()
            //        })))
            //     .ForMember(dest => dest.ChangeUserPassword, opt =>
            //        opt.MapFrom(src =>
            //        Link.To(nameof(Controllers.Site.V1.Admin.UsersController.ChangeUserPassword), new
            //        {
            //            id = src.Id,
            //            passwordForChangeDto = new PasswordForChangeDto()
            //        })));
            #endregion


            CreateMap<Photo, PhotoForUserDetailedDto>();
            CreateMap<BankCard, BankCardForUserDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForProfile , Photo>();
            CreateMap<Photo, PhotoForReturnProfileDto>();

        }
    }
}
