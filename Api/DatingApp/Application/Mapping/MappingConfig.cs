using AutoMapper;
using DatingApp.Application.DTO;
using DatingApp.Application.DTO.MemberDtos;
using DatingApp.Application.DTO.MessageDtos;
using DatingApp.Application.Extensions;
using DatingApp.Domain.Entites;

namespace DatingApp.Application.Mapping
{
    public class MappingConfig :Profile
    {

        public MappingConfig()
        {
            CreateMap<Member, MemberDto>()
           .ForMember(dest => dest.Age, opt =>
               opt.MapFrom(src => src.DateBirth.ToAge()))
           ;

            CreateMap<Photo, PhotoDto>().ReverseMap();

            CreateMap<UpdateMemberDto, Member>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember is not null));

            CreateMap<Message, MessageDto>();
        }
    }
}
