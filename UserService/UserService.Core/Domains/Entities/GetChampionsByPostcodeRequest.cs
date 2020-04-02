using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetChampionsByPostcodeRequest : IRequest<GetChampionsByPostcodeResponse>
    {
        public string PostCode { get; set; }
    }
}
