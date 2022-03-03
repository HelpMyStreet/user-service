using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.ReportService.Response;
using UserService.Core.Dto;
using HelpMyStreet.Contracts.UserService.Response;
using System.Threading;
using UserService.Core.Contracts;
using HelpMyStreet.Contracts;

namespace UserService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        Task UpdateLoginChecks(DateTime dtChecked, List<UserHistory> history);
        Task<List<User>> GetAllUsers();
        Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken);
        Task<List<UserRegistrationStep>> GetIncompleteRegistrationStatusAsync(CancellationToken cancellationToken);

        Task<List<UserDetails>> GetUserDetailsAsync(CancellationToken cancellationToken);

        List<ReportItem> GetDailyReport();

        User GetUserByID(int userId);

        List<User> GetUsersForIDs(List<int> userId);

        User GetUserByFirebaseUserID(string firebaseUID);

        List<User> GetVolunteersByPostCode(string postCode);

        Task<IReadOnlyList<User>> GetChampionsByPostCodeAsync(string postCode);

        int GetVolunteerCountByPostCode(string postCode);

        int GetChampionCountByPostCode(string postCode);

        int GetChampionPostcodesCoveredCount();

        int GetDistinctChampionUserCount();

        int GetDistinctVolunteerUserCount();

        int GetAllDistinctVolunteerUserCount();


        bool GetUserIsVerified(int userId);

        int PostCreateUser(string firebaseUserId, string emailAddress, DateTime? dateCreated, int? referringGroupID, string source);

        int ModifyUser(User user);

        int ModifyUserRegistrationPageTwo(RegistrationStepTwo registrationStepTwo);

        int ModifyUserRegistrationPageThree(RegistrationStepThree registrationStepThree);

        int ModifyUserRegistrationPageFive(RegistrationStepFive registrationStepFive);

        bool SetUserVerfication(int userId, bool isVerified);

        void CreateChampionForPostCode(int userId, string postCode);

        void CreateSupportForPostCode(int userId, string postCode);

        Task<int> GetMinUserIdAsync();

        Task<int> GetMaxUserIdAsync();


        Task<IEnumerable<VolunteerForCacheDto>> GetVolunteersForCacheAsync(int fromUserId, int toUserId);

        Task<IEnumerable<User>> GetVolunteersByIdsAsync(IEnumerable<int> userIds);

        LatitudeAndLongitudeDTO GetLatitudeAndLongitude(string postCode);
    }
}
