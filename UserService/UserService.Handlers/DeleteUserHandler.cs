using UserService.Core.Domains.Entities;
using UserService.Core.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Request;
using HelpMyStreet.Contracts.UserService.Response;
using UserService.Core.Interfaces.Services;
using Google.Apis.Upload;
using Polly.Caching;

namespace UserService.Handlers
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, DeleteUserResponse>
    {
        private readonly IRepository _repository;
        private readonly IAuthService _authService;

        public DeleteUserHandler(IRepository repository, IAuthService authService)
        {
            _repository = repository;
            _authService = authService;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            bool success = false;
            if (request.UserID>0)
            {
                var result = _repository.GetUserByID(request.UserID);
                if(result!=null && result.PostalCode==request.Postcode)
                {
                    success = _authService.DeleteUser(result.FirebaseUID).Result;

                    if(success)
                    {
                        success = await _repository.DeleteUserAsync(request.UserID, cancellationToken);
                    }
                }
            }

            return new DeleteUserResponse() { Success = success };
        }
    }
}
