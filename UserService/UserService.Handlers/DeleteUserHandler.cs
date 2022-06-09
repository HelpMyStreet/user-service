using UserService.Core.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using UserService.Core.Interfaces.Services;
using System;
using HelpMyStreet.Contracts.CommunicationService.Request;
using System.Collections.Generic;
using HelpMyStreet.Contracts.GroupService.Request;
using System.Linq;

namespace UserService.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, DeleteUserResponse>
    {
        private readonly ITrackLoginService _trackLoginService;

        public DeleteUserHandler(ITrackLoginService trackLoginService)
        {
            _trackLoginService = trackLoginService;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            return new DeleteUserResponse() 
            { 
                Success = await _trackLoginService.DeleteUser(request.UserID, request.Postcode, cancellationToken)
            };
        }
    }
}
