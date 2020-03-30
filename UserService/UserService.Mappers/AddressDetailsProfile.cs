using AutoMapper;
using UserService.Core.Dto;
using UserService.Repo.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Mappers
{
    public class AddressDetailsProfile : Profile
    {
        public AddressDetailsProfile()
        {
            CreateMap<AddressDetails, AddressDetailsDTO>();
            CreateMap<AddressDetailsDTO, AddressDetails>();
        }
    }
}
