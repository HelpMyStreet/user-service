using AutoMapper;
using Dapper;
using HelpMyStreet.Utils.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Config;
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

        public model.User GetUserByID(int userId)
        {
            User user = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Include(i => i.ChampionPostcode)
                .Where(x => x.Id == userId).FirstOrDefault();

            return MapEFUserToModelUser(user);
        }

        public model.User GetUserByFirebaseUserID(string firebaseUID)
        {
            User user = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
                .Include(i => i.ChampionPostcode)
                .Where(x => x.FirebaseUid == firebaseUID).FirstOrDefault();

            return MapEFUserToModelUser(user);
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
                UnderlyingMedicalCondition = personalDetails.UnderlyingMedicalCondition,
                Address = new model.Address()
                {
                    AddressLine1 = personalDetails.AddressLine1,
                    AddressLine2 = personalDetails.AddressLine2,
                    AddressLine3 = personalDetails.AddressLine3,
                    Locality = personalDetails.Locality,
                    Postcode = personalDetails.Postcode
                }
            };
        }

        private PersonalDetails MapModelPersonalDetailsToEFPersonalDetails(model.UserPersonalDetails userPersonalDetails)
        {
            return new PersonalDetails()
            {
                FirstName = userPersonalDetails.FirstName,
                LastName = userPersonalDetails.LastName,
                DisplayName = userPersonalDetails.DisplayName,
                DateOfBirth = userPersonalDetails.DateOfBirth,
                EmailAddress = userPersonalDetails.EmailAddress,
                MobilePhone = userPersonalDetails.MobilePhone,
                OtherPhone = userPersonalDetails.OtherPhone,
                Postcode = userPersonalDetails.Address.Postcode,
                AddressLine1 = userPersonalDetails.Address.AddressLine1,
                AddressLine2 = userPersonalDetails.Address.AddressLine2,
                AddressLine3 = userPersonalDetails.Address.AddressLine3,
                Locality = userPersonalDetails.Address.Locality
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

        private User MapModelUserToEFUser(model.User user)
        {
            return new User()
            {
                DateCreated = user.DateCreated,
                FirebaseUid = user.FirebaseUID,
                EmailSharingConsent = user.EmailSharingConsent,
                HmscontactConsent = user.HMSContactConsent,
                IsVerified = user.IsVerified,
                IsVolunteer = user.IsVolunteer,
                MobileSharingConsent = user.MobileSharingConsent,
                OtherPhoneSharingConsent = user.OtherPhoneSharingConsent,
                PostalCode = user.PostalCode,
                PersonalDetails = MapModelPersonalDetailsToEFPersonalDetails(user.UserPersonalDetails)
            };
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

        public int ModifyUserRegistrationPageTwo(model.RegistrationStepTwo registrationStepTwo)
        {
            User EFUser = _context.User
                .Include(i => i.PersonalDetails)
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
                _context.SaveChanges();
            }
            return registrationStepTwo.UserID;
        }

        public int ModifyUserRegistrationPageThree(model.RegistrationStepThree registrationStepThree)
        {
            User EFUser = _context.User
                .Include(i => i.PersonalDetails)
                .Include(i => i.SupportActivity)
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
                _context.SaveChanges();
            }
            return registrationStepThree.UserID;
        }

        public int ModifyUserRegistrationPageFour(model.RegistrationStepFour registrationStepFour)
        {
            User EFUser = _context.User
                .Include(i => i.ChampionPostcode)
                .Where(a => a.Id == registrationStepFour.UserID).FirstOrDefault();

            if (EFUser != null)
            {
                EFUser.StreetChampionRoleUnderstood = registrationStepFour.StreetChampionRoleUnderstood;

                _context.ChampionPostcode.RemoveRange(EFUser.ChampionPostcode);

                foreach (string cp in registrationStepFour.ChampionPostcodes)
                {
                    _context.ChampionPostcode.Add(new ChampionPostcode()
                    {
                        User = EFUser,
                        PostalCode = cp
                    });
                }

                _context.SaveChanges();
            }
            return registrationStepFour.UserID;
        }

        public int ModifyUserRegistrationPageFive(model.RegistrationStepFive registrationStepFive)
        {
            User EFUser = _context.User
               .Where(a => a.Id == registrationStepFive.UserID).FirstOrDefault();

            if (EFUser != null)
            {
                EFUser.IsVerified = registrationStepFive.IsVerified;
                _context.SaveChanges();
            }
            return registrationStepFive.UserID;
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


        public async Task<IEnumerable<HelperPostcodeRadiusDto>> GetAllVolunteersPostcodeRadiiAsync()
        {
            var query = @"
SELECT [ID] AS [UserId], 
[PostalCode] AS [Postcode], 
[SupportRadiusMiles]
FROM [User].[User]
WHERE 
[SupportRadiusMiles] IS NOT NULL AND 
[IsVolunteer] = 1 
";

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.SqlConnectionString))
            {
                IEnumerable<HelperPostcodeRadiusDto> result = await connection.QueryAsync<HelperPostcodeRadiusDto>(query,
                    commandType: CommandType.Text,
                    commandTimeout: 15);

                return result;
            }

        }

        public async Task<IEnumerable<HelperPostcodeRadiusDto>> GetVolunteersPostcodeRadiiAsync(int fromUserId, int toUserId)
        {
            var query = @"
SELECT [ID] AS [UserId], 
[PostalCode] AS [Postcode], 
[SupportRadiusMiles]
FROM [User].[User]
WHERE 
[SupportRadiusMiles] IS NOT NULL AND 
[IsVolunteer] = 1 AND
[ID] >= @FromUserId AND 
[ID] <= @ToUser1Id
";

            using (SqlConnection connection = new SqlConnection(_connectionStrings.Value.SqlConnectionString))
            {
                IEnumerable<HelperPostcodeRadiusDto> result = await connection.QueryAsync<HelperPostcodeRadiusDto>(query,
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
                .Include(x=>x.ChampionPostcode)
                .Include(x=>x.SupportActivity)
                .Include(x=>x.PersonalDetails)
                .ToListAsync();


            List<model.User> response = new List<model.User>();
            foreach (var user in users)
            {
                response.Add(MapEFUserToModelUser(user));
            }

            return response;
        }
    }
}
