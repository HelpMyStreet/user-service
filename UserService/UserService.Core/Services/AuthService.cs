using System;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;
using UserService.Core.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly FirebaseAuth _firebase;
        private readonly IOptions<FirebaseConfig> _firebaseConfig;

        public AuthService(IOptions<FirebaseConfig> firebaseConfig)
        {
            _firebaseConfig = firebaseConfig;
            var firebaseCredentials = _firebaseConfig.Value.Credentials;
            if (firebaseCredentials == string.Empty)
            {
                throw new Exception("Firebase cedentials missing");
            }
            var fb = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(firebaseCredentials)
            });

            _firebase = FirebaseAuth.GetAuth(fb);


        }

        public async Task<bool> DeleteUser(string firebaseUserID)
        {
            try
            {
                await _firebase.DeleteUserAsync(firebaseUserID);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<List<UserHistory>> GetHistoryForUsers(List<string> firebaseUserIds)
        {
            var users = await _firebase.GetUsersAsync(firebaseUserIds.Select(x => new UidIdentifier(x)).ToList());

            List<UserHistory> history = new List<UserHistory>();

            if (users!=null)
            {
                users.Users.ToList().ForEach(user => history.Add(new UserHistory()
                {
                    FirebaseUserId = user.Uid,
                    EmailAddress = user.Email,
                    CreationTimestamp = user.UserMetaData.CreationTimestamp,
                    LastSignInTimestamp = user.UserMetaData.LastSignInTimestamp
                }));
            }

            return history;
        }
    }
}
