﻿using UserService.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        Task AddPostCode(PostCodeDTO postCodeDTO);
        Task AddAddress(AddressDetailsDTO addressDetailsDTO);
    }
}
