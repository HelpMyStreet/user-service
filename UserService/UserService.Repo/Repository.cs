using AutoMapper;
using Dapper;
using HelpMyStreet.Contracts.ReportService.Response;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserService.Core.Config;
using UserService.Core.Domains.Entities;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Repo.EntityFramework.Entities;
using model = HelpMyStreet.Utils.Models;

namespace UserService.Repo
{

    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IOptionsSnapshot<ConnectionStrings> _connectionStrings;

        public Repository(ApplicationDbContext context, IMapper mapper, IOptionsSnapshot<ConnectionStrings> connectionStrings)
        {
            _context = context;
            _mapper = mapper;
            _connectionStrings = connectionStrings;
        }

        private enum RegistrationSteps : byte
        {
            StepOne = 1,
            StepTwo,
            StepThree,
            StepFour,
            StepFive
        }


        public model.User GetUserByID(int userId)
        {
            User user = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Include(i => i.ChampionPostcode)
                .Include(i => i.RegistrationHistory)
                .Where(x => x.Id == userId).FirstOrDefault();

            return MapEFUserToModelUser(user);
        }

        public model.User GetUserByFirebaseUserID(string firebaseUID)
        {
            User user = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Include(i => i.ChampionPostcode)
                .Include(i => i.RegistrationHistory)
                .Where(x => x.FirebaseUid == firebaseUID).FirstOrDefault();

            return MapEFUserToModelUser(user);
        }

        public List<model.User> GetUsersForIDs(List<int> userId)
        {
            List<User> users = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Include(i => i.ChampionPostcode)
                .Include(i => i.RegistrationHistory)
                .Where(p => userId.Contains(p.Id))
                .ToList();

            List<model.User> response = new List<model.User>();
            foreach (User user in users)
            {
                response.Add(MapEFUserToModelUser(user));
            }

            return response;
        }

        public bool GetUserIsVerified(int userId)
        {
            var user = _context.User.Where(x => x.Id == userId).FirstOrDefault();

            if (user != null)
            {
                if (user.IsVerified.HasValue)
                {
                    return user.IsVerified.Value;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<model.User> GetVolunteersByPostCode(string postCode)
        {
            List<SupportPostcode> users = _context.SupportPostcode
                .Include(i => i.User.PersonalDetails)
                .Where(x => x.PostalCode == postCode.ToLower()
                    && x.User.IsVerified.Value == true)
                .ToList();

            List<model.User> response = new List<model.User>();
            foreach (SupportPostcode sp in users)
            {
                response.Add(MapEFUserToModelUser(sp.User));
            }

            return response;
        }

        public async Task<IReadOnlyList<model.User>> GetChampionsByPostCodeAsync(string postCode)
        {
            IEnumerable<ChampionPostcode> users = await _context.ChampionPostcode
                .Include(i => i.User.PersonalDetails)
                .Include(i => i.User.ChampionPostcode)
                .Include(i => i.User.RegistrationHistory)
                .Include(i => i.User.SupportActivity)
                .Where(x => x.PostalCode == postCode
                            && x.User.IsVerified.Value == true)
                .ToListAsync();

            List<model.User> response = new List<model.User>();
            foreach (ChampionPostcode cp in users)
            {
                response.Add(MapEFUserToModelUser(cp.User));
            }

            return response;
        }

        public int GetVolunteerCountByPostCode(string postCode)
        {
            return _context.SupportPostcode
                .Count(x => x.PostalCode == postCode && x.User.IsVerified.Value == true);
        }

        public int GetChampionCountByPostCode(string postCode)
        {
            return _context.ChampionPostcode
                .Count(x => x.PostalCode == postCode && x.User.IsVerified.Value == true);
        }

        public int PostCreateUser(string firebaseUserId, string emailAddress, DateTime? dateCreated)
        {
            User user = new User()
            {
                FirebaseUid = firebaseUserId,
                DateCreated = dateCreated,
                PersonalDetails = new PersonalDetails()
                {
                    EmailAddress = emailAddress
                }
            };
            _context.User.Add(user);
            AddRegistrationHistoryForUser(user, RegistrationSteps.StepOne);
            _context.SaveChanges();
            return user.Id;
        }

        private List<SupportActivities> GetSupportActivities(ICollection<SupportActivity> activities)
        {
            var response = new List<SupportActivities>();
            foreach (SupportActivity supportActivity in activities)
            {
                response.Add((SupportActivities)supportActivity.ActivityId);
            }

            return response;
        }

        private List<string> GetChampionPostCodes(ICollection<ChampionPostcode> postcodes)
        {
            var response = new List<string>();
            foreach (ChampionPostcode championPostcode in postcodes)
            {
                response.Add(championPostcode.PostalCode);
            }
            return response;
        }

        private Dictionary<int, DateTime> GetRegistrationHistories(ICollection<RegistrationHistory> registrationHistories)
        {
            var response = new Dictionary<int, DateTime>();

            foreach (RegistrationHistory registrationHistory in registrationHistories)
            {
                response.Add((int)registrationHistory.RegistrationStep, registrationHistory.DateCompleted);
            }

            return response;
        }


        private model.User MapEFUserToModelUser(User user)
        {
            return new model.User()
            {
                DateCreated = user.DateCreated.Value,
                ID = user.Id,
                FirebaseUID = user.FirebaseUid,
                EmailSharingConsent = user.EmailSharingConsent,
                HMSContactConsent = user.HmscontactConsent,
                IsVerified = user.IsVerified,
                IsVolunteer = user.IsVolunteer,
                MobileSharingConsent = user.MobileSharingConsent,
                OtherPhoneSharingConsent = user.OtherPhoneSharingConsent,
                PostalCode = user.PostalCode,
                SupportRadiusMiles = (float?)user.SupportRadiusMiles,
                StreetChampionRoleUnderstood = user.StreetChampionRoleUnderstood,
                SupportVolunteersByPhone = user.SupportVolunteersByPhone,
                SupportActivities = GetSupportActivities(user.SupportActivity),
                ChampionPostcodes = GetChampionPostCodes(user.ChampionPostcode),
                RegistrationHistory = GetRegistrationHistories(user.RegistrationHistory),
                UserPersonalDetails = MapEFPersonalDetailsToModelPersonalDetails(user.PersonalDetails)
            };
        }

        private model.UserPersonalDetails MapEFPersonalDetailsToModelPersonalDetails(PersonalDetails personalDetails)
        {
            return new model.UserPersonalDetails()
            {
                FirstName = personalDetails.FirstName,
                LastName = personalDetails.LastName,
                DisplayName = personalDetails.DisplayName,
                DateOfBirth = personalDetails.DateOfBirth,
                EmailAddress = personalDetails.EmailAddress,
                MobilePhone = personalDetails.MobilePhone,
                OtherPhone = personalDetails.OtherPhone,
                Address = new model.Address()
                {
                    AddressLine1 = personalDetails.AddressLine1,
                    AddressLine2 = personalDetails.AddressLine2,
                    AddressLine3 = personalDetails.AddressLine3,
                    Locality = personalDetails.Locality,
                    Postcode = personalDetails.Postcode
                },
                UnderlyingMedicalCondition = personalDetails.UnderlyingMedicalCondition
            };
        }

        private void UpdateEFPersonalDetailsFromModelPersonalDetails(model.UserPersonalDetails userPersonalDetails, PersonalDetails EFPersonalDetails)
        {
            EFPersonalDetails.FirstName = userPersonalDetails.FirstName;
            EFPersonalDetails.LastName = userPersonalDetails.LastName;
            EFPersonalDetails.DisplayName = userPersonalDetails.DisplayName;
            EFPersonalDetails.DateOfBirth = userPersonalDetails.DateOfBirth;
            EFPersonalDetails.EmailAddress = userPersonalDetails.EmailAddress;
            EFPersonalDetails.MobilePhone = userPersonalDetails.MobilePhone;
            EFPersonalDetails.OtherPhone = userPersonalDetails.OtherPhone;
            EFPersonalDetails.Postcode = userPersonalDetails.Address.Postcode;
            EFPersonalDetails.AddressLine1 = userPersonalDetails.Address.AddressLine1;
            EFPersonalDetails.AddressLine2 = userPersonalDetails.Address.AddressLine2;
            EFPersonalDetails.AddressLine3 = userPersonalDetails.Address.AddressLine3;
            EFPersonalDetails.Locality = userPersonalDetails.Address.Locality;
            EFPersonalDetails.UnderlyingMedicalCondition = userPersonalDetails.UnderlyingMedicalCondition;
        }
        private void UpdateEFUserFromUserModel(model.User user, User EFUser)
        {
            EFUser.FirebaseUid = user.FirebaseUID;
            EFUser.EmailSharingConsent = user.EmailSharingConsent;
            EFUser.HmscontactConsent = user.HMSContactConsent;
            EFUser.IsVerified = user.IsVerified;
            EFUser.IsVolunteer = user.IsVolunteer;
            EFUser.MobileSharingConsent = user.MobileSharingConsent;
            EFUser.OtherPhoneSharingConsent = user.OtherPhoneSharingConsent;
            EFUser.PostalCode = user.PostalCode;
            EFUser.StreetChampionRoleUnderstood = user.StreetChampionRoleUnderstood;
            EFUser.SupportRadiusMiles = user.SupportRadiusMiles;
            EFUser.SupportVolunteersByPhone = user.SupportVolunteersByPhone;
            UpdateEFPersonalDetailsFromModelPersonalDetails(user.UserPersonalDetails, EFUser.PersonalDetails);
        }

        public void CreateChampionForPostCode(int userId, string postCode)
        {
            var user = _context.User.Where(a => a.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var result = _context.ChampionPostcode.Where(a => a.User == user && a.PostalCode == postCode).FirstOrDefault();

                if (result == null)
                {
                    _context.ChampionPostcode.Add(new ChampionPostcode()
                    {
                        User = user,
                        PostalCode = postCode
                    });
                    _context.SaveChanges();
                }
            }
        }

        public void CreateSupportForPostCode(int userId, string postCode)
        {
            var user = _context.User.Where(a => a.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var result = _context.SupportPostcode.Where(a => a.User == user && a.PostalCode == postCode).FirstOrDefault();

                if (result == null)
                {
                    _context.SupportPostcode.Add(new SupportPostcode()
                    {
                        User = user,
                        PostalCode = postCode
                    });
                    _context.SaveChanges();
                }
            }
        }

        public bool SetUserVerfication(int userId, bool isVerified)
        {
            var user = _context.User.Where(a => a.Id == userId).FirstOrDefault();

            if (user != null)
            {
                user.IsVerified = isVerified;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public int ModifyUser(model.User user)
        {
            User EFUser = _context.User
                .Include(i => i.PersonalDetails)
                .Where(a => a.Id == user.ID).FirstOrDefault();

            if (EFUser != null)
            {

                UpdateEFUserFromUserModel(user, EFUser);
                _context.SaveChanges();
                return EFUser.Id;
            }
            else
            {
                return -1;
            }
        }

        public int GetChampionPostcodesCoveredCount()
        {
            return _context.ChampionPostcode.Count(x => x.User.IsVerified == true);
        }

        public int GetDistinctChampionUserCount()
        {
            return _context.ChampionPostcode.Select(x => x.User)
                .Where(x => x.IsVerified == true)
                .Distinct()
                .Count();
        }

        public int GetDistinctVolunteerUserCount()
        {
            return _context.User.Where(x => x.IsVerified == true/* && x.IsVolunteer == true*/)
                .Distinct()
                .Count();
        }

        public int GetAllDistinctVolunteerUserCount()
        {
            return _context.User
                .Distinct()
                .Count();
        }

        public int ModifyUserRegistrationPageTwo(model.RegistrationStepTwo registrationStepTwo)
        {
            User EFUser = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.RegistrationHistory)
                .Where(a => a.Id == registrationStepTwo.UserID).FirstOrDefault();

            if (EFUser != null)
            {
                EFUser.PostalCode = registrationStepTwo.PostalCode;
                EFUser.PersonalDetails.FirstName = registrationStepTwo.FirstName;
                EFUser.PersonalDetails.LastName = registrationStepTwo.LastName;
                EFUser.PersonalDetails.MobilePhone = registrationStepTwo.MobilePhone;
                EFUser.PersonalDetails.OtherPhone = registrationStepTwo.OtherPhone;
                EFUser.PersonalDetails.DateOfBirth = registrationStepTwo.DateOfBirth;
                EFUser.PersonalDetails.DisplayName = registrationStepTwo.DisplayName;
                EFUser.PersonalDetails.AddressLine1 = registrationStepTwo.Address.AddressLine1;
                EFUser.PersonalDetails.AddressLine2 = registrationStepTwo.Address.AddressLine2;
                EFUser.PersonalDetails.AddressLine3 = registrationStepTwo.Address.AddressLine3;
                EFUser.PersonalDetails.Locality = registrationStepTwo.Address.Locality;
                EFUser.PersonalDetails.Postcode = registrationStepTwo.Address.Postcode;
                AddRegistrationHistoryForUser(EFUser, RegistrationSteps.StepTwo);
                _context.SaveChanges();
            }
            return registrationStepTwo.UserID;
        }

        public int ModifyUserRegistrationPageThree(model.RegistrationStepThree registrationStepThree)
        {
            User EFUser = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Include(i => i.RegistrationHistory)
                .Where(a => a.Id == registrationStepThree.UserID).FirstOrDefault();

            if (EFUser != null)
            {
                EFUser.SupportRadiusMiles = registrationStepThree.SupportRadiusMiles;
                EFUser.SupportVolunteersByPhone = registrationStepThree.SupportVolunteersByPhone;
                EFUser.PersonalDetails.UnderlyingMedicalCondition = registrationStepThree.UnderlyingMedicalCondition;

                _context.SupportActivity.RemoveRange(EFUser.SupportActivity);

                foreach (HelpMyStreet.Utils.Enums.SupportActivities sa in registrationStepThree.Activities)
                {
                    _context.SupportActivity.Add(new SupportActivity
                    {
                        User = EFUser,
                        ActivityId = (byte)sa
                    });
                }
                EFUser.IsVolunteer = true;
                AddRegistrationHistoryForUser(EFUser, RegistrationSteps.StepThree);
                _context.SaveChanges();
            }
            return registrationStepThree.UserID;
        }

        public int ModifyUserRegistrationPageFour(model.RegistrationStepFour registrationStepFour)
        {
            User EFUser = _context.User
                .Include(i => i.ChampionPostcode)
                .Include(i => i.RegistrationHistory)
                .Where(a => a.Id == registrationStepFour.UserID).FirstOrDefault();

            if (EFUser != null)
            {
                _context.ChampionPostcode.RemoveRange(EFUser.ChampionPostcode);

                EFUser.StreetChampionRoleUnderstood = registrationStepFour.StreetChampionRoleUnderstood;

                if (EFUser.StreetChampionRoleUnderstood.HasValue && EFUser.StreetChampionRoleUnderstood.Value == true)
                {
                    foreach (string cp in registrationStepFour.ChampionPostcodes)
                    {
                        _context.ChampionPostcode.Add(new ChampionPostcode()
                        {
                            User = EFUser,
                            PostalCode = cp
                        });
                    }
                }
                AddRegistrationHistoryForUser(EFUser, RegistrationSteps.StepFour);
                _context.SaveChanges();
            }
            return registrationStepFour.UserID;
        }

        public int ModifyUserRegistrationPageFive(model.RegistrationStepFive registrationStepFive)
        {
            User EFUser = _context.User
                .Include(i => i.RegistrationHistory)
                .Where(a => a.Id == registrationStepFive.UserID).FirstOrDefault();

            if (EFUser != null)
            {
                EFUser.IsVerified = registrationStepFive.IsVerified;
                AddRegistrationHistoryForUser(EFUser, RegistrationSteps.StepFive);
                _context.SaveChanges();
            }
            return registrationStepFive.UserID;
        }

        private void AddRegistrationHistoryForUser(User user, RegistrationSteps registrationStep)
        {
            var regHistory = user.RegistrationHistory.FirstOrDefault(w => w.RegistrationStep == (byte)registrationStep);

            if (regHistory != null)
            {
                user.RegistrationHistory.Remove(regHistory);
            }
            user.RegistrationHistory.Add(new RegistrationHistory()
            {
                DateCompleted = DateTime.Now.ToUniversalTime(),
                RegistrationStep = (byte)registrationStep,
                UserId = user.Id
            });
        }

        public async Task<int> GetMinUserIdAsync()
        {
            int userId = await _context.User.MinAsync(x => x.Id);

            return userId;
        }
        public async Task<int> GetMaxUserIdAsync()
        {
            int userId = await _context.User.MaxAsync(x => x.Id);

            return userId;
        }


        public async Task<IEnumerable<VolunteerForCacheDto>> GetVolunteersForCacheAsync(int fromUserId, int toUserId)
        {
            var query = @"
;WITH StreetChampions as(
SELECT [ID] AS [UserId] 
FROM [User].[User] u
INNER JOIN [User].[ChampionPostcode] cp
ON u.ID = cp.UserID
WHERE [StreetChampionRoleUnderstood] = 1
GROUP BY [ID]
)

SELECT u.[ID] AS [UserId], 
[PostalCode] AS [Postcode], 
pc.Longitude,
pc.Latitude,
[SupportRadiusMiles],
IIF(u.[IsVerified] = 1, 1, 2) [IsVerifiedType],
IIF(sc.UserId IS NOT NULL, 2, 1) as [VolunteerType]
FROM [User].[User] u
LEFT JOIN StreetChampions sc
on u.ID = sc.UserId
INNER JOIN Address.Postcode pc on u.PostalCode = pc.Postcode
WHERE 
u.[SupportRadiusMiles] IS NOT NULL AND 
u.[IsVolunteer] = 1  AND
u.[ID] >= @FromUserId AND 
u.[ID] <= @ToUser1Id
";

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.SqlConnectionString))
            {
                if (connection.DataSource.Contains("database.windows.net"))
                {
                    connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
                }
                IEnumerable<VolunteerForCacheDto> result = await connection.QueryAsync<VolunteerForCacheDto>(query,
                    commandType: CommandType.Text,
                    param: new { FromUserId = fromUserId, ToUser1Id = toUserId },
                    commandTimeout: 15);

                return result;
            }
        }

        public async Task<IEnumerable<model.User>> GetVolunteersByIdsAsync(IEnumerable<int> userIds)
        {            
            var users = await _context.User
                .Where(x => x.IsVolunteer == true         
                            && userIds.Contains(x.Id))
                .Include(x => x.ChampionPostcode)
                .Include(x => x.SupportActivity)
                .Include(x => x.PersonalDetails)
                .Include(x => x.RegistrationHistory)
                .ToListAsync();

            List<model.User> response = new List<model.User>();
            foreach (var user in users)
            {
                response.Add(MapEFUserToModelUser(user));
            }

            return response;
        }


        public List<ReportItem> GetDailyReport()
        {
            List<ReportItem> response = new List<ReportItem>();
            List<DailyReport> result = _context.DailyReport.ToList();

            if (result != null)
            {
                foreach (DailyReport dailyReport in result)
                {
                    response.Add(new ReportItem()
                    {
                        Section = dailyReport.Section,
                        Last2Hours = dailyReport.Last2Hours,
                        Today = dailyReport.Today,
                        SinceLaunch = dailyReport.SinceLaunch
                    }) ;
                }
            }

            return response;
        }

        public async  Task<List<UserDetails>> GetUserDetailsAsync(CancellationToken cancellationToken)
        {
            return await _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Select(u => new UserDetails
            {
                UserID = u.Id,
                IsVerified = u.IsVerified.Value,
                IsVolunteer = u.IsVolunteer.Value,
                IsStreetChampion = u.StreetChampionRoleUnderstood.Value,
                FirstName = u.PersonalDetails.FirstName,
                LastName = u.PersonalDetails.LastName,
                EmailAddress = u.PersonalDetails.EmailAddress,
                PostCode = u.PostalCode,
                SupportActivities = GetSupportActivities(u.SupportActivity),
                SupportRadiusMiles = u.SupportRadiusMiles.Value
                }).ToListAsync(cancellationToken);
        }

        public LatitudeAndLongitudeDTO GetLatitudeAndLongitude(string postCode)
        {
            var postcodeDetails =  _context.Postcode.Where(x => x.Postcode == postCode).FirstOrDefault();
            if (postcodeDetails != null)
            {
                return new LatitudeAndLongitudeDTO()
                {
                    Latitude = Convert.ToDouble(postcodeDetails.Latitude),
                    Longitude = Convert.ToDouble(postcodeDetails.Longitude)
                };
            }
            else
            {
                throw new Exception($"Cannot find longitude and latitude for {postCode}");
            }
        }
    }
}
