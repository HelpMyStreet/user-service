using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreet.Contracts.ReportService.Response;

namespace UserService.Core.Domains.Entities
{
    public class GetReportRequest : IRequest<GetReportResponse>
    {
    }
}
