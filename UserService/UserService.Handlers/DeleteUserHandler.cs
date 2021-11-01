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
        private readonly IRepository _repository;
        private readonly IAuthService _authService;
        private readonly ICommunicationService _communicationService;
        private readonly IGroupService _groupService;

        public DeleteUserHandler(IRepository repository, IAuthService authService, ICommunicationService communicationService, IGroupService groupService)
        {
            _repository = repository;
            _authService = authService;
            _communicationService = communicationService;
            _groupService = groupService;
        }

        public async Task<DeleteUserResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            bool success = false;
            int groupId;
            if (request.UserID>0)
            {
                var result = _repository.GetUserByID(request.UserID);

                if(result!=null && result.PostalCode==request.Postcode)
                {
                    success = _authService.DeleteUser(result.FirebaseUID).Result;

                    if(success)
                    {
                        var groupRoles = await _groupService.GetUserRoles(request.UserID, cancellationToken);

                        foreach (KeyValuePair<int, List<int>> group in groupRoles)
                        {
                            groupId = group.Key;
                            foreach(int role in group.Value.OrderByDescending(x => x))
                            {
                                await _groupService.PostRevokeRole(new PostRevokeRoleRequest()
                                {
                                    GroupID = group.Key,
                                    AuthorisedByUserID = -1,
                                    UserID = request.UserID,
                                    Role = new RoleRequest()
                                    {
                                        GroupRole = (HelpMyStreet.Utils.Enums.GroupRoles) role
                                    }
                                },cancellationToken);
                            }
                        }

                        success = await _repository.DeleteUserAsync(request.UserID, cancellationToken);
                        bool deletedMarketingContact = await _communicationService.DeleteMarketingContactAsync(new DeleteMarketingContactRequest()
                        {
                            EmailAddress = result.UserPersonalDetails.EmailAddress
                        },cancellationToken);

                        if(!deletedMarketingContact)
                        {
                            throw new Exception("Unable to delete email address from marketing list");
                        }
                    }
                }
            }

            return new DeleteUserResponse() { Success = success };
        }
    }
}
