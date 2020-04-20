using UserService.Core.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using HelpMyStreet.Utils.Models;

namespace UserService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        User GetUserByID(int userId);

        User GetUserByFirebaseUserID(string firebaseUID);

        List<User> GetVolunteersByPostCode(string postCode);

        Task<IReadOnlyList<User>> GetChampionsByPostCodeAsync(string postCode);

        int GetVolunteerCountByPostCode(string postCode);

        int GetChampionCountByPostCode(string postCode);

        int GetChampionPostcodesCoveredCount();

        int GetDistinctChampionUserCount();

        int GetDistinctVolunteerUserCount();

        bool GetUserIsVerified(int userId);

        int PostCreateUser(string firebaseUserId, string emailAddress, DateTime? dateCreated);

        int ModifyUser(User user);

        int ModifyUserRegistrationPageTwo(RegistrationStepTwo registrationStepTwo);

        int ModifyUserRegistrationPageThree(RegistrationStepThree registrationStepThree);

        int ModifyUserRegistrationPageFour(RegistrationStepFour registrationStepFour);

        int ModifyUserRegistrationPageFive(RegistrationStepFive registrationStepFive);

        bool SetUserVerfication(int userId, bool isVerified);

        void CreateChampionForPostCode(int userId, string postCode);

        void CreateSupportForPostCode(int userId, string postCode);

        Task<int> GetMinUserIdAsync();

        Task<int> GetMaxUserIdAsync();

        Task<IEnumerable<HelperPostcodeRadiusDto>> GetAllVolunteersPostcodeRadiiAsync();

        Task<IEnumerable<HelperPostcodeRadiusDto>> GetVolunteersPostcodeRadiiAsync(int fromUserId, int toUserId);

        Task<IEnumerable<User>> GetVolunteersByIdsAsync(IEnumerable<int> userIds);
    }
}
