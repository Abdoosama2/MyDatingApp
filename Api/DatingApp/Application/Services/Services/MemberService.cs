using AutoMapper;
using DatingApp.Application.DTO;
using DatingApp.Application.DTO.MemberDtos;
using DatingApp.Application.Execptions;
using DatingApp.Application.Helpers;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Domain.Entites;
using DatingApp.Domain.Interfaces;

namespace DatingApp.Application.Services.Services
{
  
        public class MemberService(IUnitOfWork uow,IPhotoService photoService, IMapper mapper) : IMemberService
        {
            public async Task<PaginatedResult<MemberDto>> GetMembersAsync(MemberParams memberParams)
            {
                var members = await uow.memberRepository.GetMembersAsync(memberParams);
            return new PaginatedResult<MemberDto>
            {
                Items = mapper.Map<List<MemberDto>>(members.Items),
                MetaData = members.MetaData
            };
        }

            public async Task<MemberDto?> GetMemberByIdAsync(string id)
            {
                var member = await uow.memberRepository.GetMemberByIdAsync(id);
                if (member is null) throw new NotFoundException(nameof(member), id);
                return mapper.Map<MemberDto>(member);
            }

            public async Task<IReadOnlyList<PhotoDto>> GetMemberPhotosAsync(string memberId)
            {
                var photos = await uow.memberRepository.GetPhotoForMemberAsync(memberId);
                return mapper.Map<IReadOnlyList<PhotoDto>>(photos);
            }

            public async Task<bool> UpdateMemberAsync(string memberId, UpdateMemberDto updateDto)
            {
                var member = await uow.memberRepository.GetMemberForUpdate(memberId);
                if (member is null) throw new NotFoundException(nameof(member), memberId);

                mapper.Map(updateDto, member);
            member.User.DisplayName = updateDto.DisplayName??member.User.DisplayName;

            uow.memberRepository.Update(member);

                return await uow.Complete();
            }

        public async Task<Member> GetMemberForUpdate(string memberId)
        {
            var member= await uow.memberRepository.GetMemberForUpdate(memberId);
            if (member == null)
            {
                throw new NotFoundException(nameof(member), memberId);
            }
            return member;
        }

        public async Task<bool> UploadMemberPhotoAsync(string memberId, PhotoDto photo)
        {
            var member = await uow.memberRepository.GetMemberForUpdate(memberId);
            if (member == null)
            {
                throw new NotFoundException(nameof(member), memberId);
            }
            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }
             var newPhoto = mapper.Map<Photo>(photo);
             member.Photos.Add(newPhoto);

            if( await uow.Complete())
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeMemberMainPhoto(string memberId, int photoId)
        {
            var member = await uow.memberRepository.GetMemberForUpdate(memberId);
            if (member == null)
            {
                throw new NotFoundException(nameof(member), memberId);
            }
            var memberPhoto = member.Photos.FirstOrDefault(x => x.Id == photoId);
            if (memberPhoto == null || memberPhoto.Url == member.ImageUrl  )
            {
                return false;
            }

            member.ImageUrl = memberPhoto.Url;
            member.User.ImageUrl = memberPhoto.Url;
            if (await uow.Complete())
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteMemberPhoto(string memberId, int photoId)
        {
            var member = await uow.memberRepository.GetMemberForUpdate(memberId);
            if (member == null)
            {
                throw new NotFoundException(nameof(member), memberId);
            }
            var memberPhoto = await uow.memberRepository.GetMemberPhotoById(memberId, photoId);
            if (memberPhoto == null)
            {
                return false;
            }
            if (memberPhoto.Url == member.User.ImageUrl)
            {
                return false;
            }
            var result = await photoService.DeletePhotoAsync(memberPhoto.PublicId);
            if (result.Error != null)
            {
                return false;
            }

           
            member.Photos.Remove(memberPhoto);
            if (await uow.Complete())
            {
                return true;
            }
            return false;

        }
    }
    }

