using HelpMyStreet.Utils.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Domains.Entities
{
    public class GetHelpersByPostcodeAndTaskTypeRequest : IRequest<GetHelpersByPostcodeAndTaskTypeResponse>
    {
        public string Postcode { get; set; }
        public TasksRequested RequestedTasks {get; set; }
    }

    public class TasksRequested
    {
        public List<SupportActivities> SupportActivities { get; set; }
    }
}
