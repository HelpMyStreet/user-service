using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class PostCreateChampionForPostCodeRequest : IRequest
    {
        public string UserID { get; set; }
        public string PostCode { get; set; }
    }
}
