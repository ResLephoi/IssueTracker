using AutoMapper;
using IssueTracker.Domain.DTOs;
using IssueTracker.Domain.Entities;

namespace IssueTracker.Application.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateItemDTO, Item>();
            CreateMap<CreateCardDTO, Card>();
        }
    }
}