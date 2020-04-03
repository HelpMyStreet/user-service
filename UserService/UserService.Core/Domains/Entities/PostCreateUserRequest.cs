using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class PostCreateUserRequest : IRequest<PostCreateUserResponse>
    {
        public string FirebaseUID { get; set; }
        public string EmailAddress { get; set; }
    }
}
