using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class PostCreateSupportForPostCodeRequest : IRequest
    {
        public int UserID { get; set; }
        public string PostCode { get; set; }
    }
}
