using AutoMapper;
using Domain.DTOs;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateItemDTO, Item>();
        }
    }
}