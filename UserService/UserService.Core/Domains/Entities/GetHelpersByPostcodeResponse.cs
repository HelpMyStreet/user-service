using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetHelpersByPostcodeResponse
    {
        public List<User> Users { get; set; }
    }
}
