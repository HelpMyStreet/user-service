using System;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Interfaces.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;

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
    }
}
