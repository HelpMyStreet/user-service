using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetChampionCountByPostcodeRequest : IRequest<GetChampionCountByPostcodeResponse>
    {
        public string PostCode { get; set; }
    }
}
