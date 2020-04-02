using AutoMapper;
using UserService.Core.Dto;
using UserService.Core.Interfaces.Repositories;
using UserService.Repo.EntityFramework.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Domains.Entities;
using model = HelpMyStreet.Utils.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UserService.Repo
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Repository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public model.User GetUserByID(string userId)
        {
            User user = _context.User
                .Include(i => i.PersonalDetails)
                .Where(x => x.Id.ToString() == userId).FirstOrDefault();

            return MapEFUserToModelUser(user);
        }

        public bool GetUserIsVerified(string userId)
        {
            var user = _context.User.Where(x => x.Id.ToString() == userId).FirstOrDefault();

            if(user!=null)
            {
                return user.IsVerified.Value;
            }
            else
            {
                return false;
            }
        }

        public List<model.User> GetVolunteersByPostCode(string postCode)
        {
            List<SupportPostcode> users = _context.SupportPostcode
                .Where(x => x.PostalCode == postCode.ToLower()
                    && x.User.IsVerified.Value == true)
                .ToList();

            List<model.User> response = new List<model.User>();
            foreach(SupportPostcode sp in users)
            {
                response.Add(MapEFUserToModelUser(sp.User));
            }

            return response;
        }

        public List<model.User> GetChampionsByPostCode(string postCode)
        {
            List<ChampionPostcode> users = _context.ChampionPostcode
                .Where(x => x.PostalCode == postCode.ToLower() 
                        && x.User.IsVerified.Value==true)
                .ToList();

            List<model.User> response = new List<model.User>();
            foreach (ChampionPostcode cp in users)
            {
                response.Add(MapEFUserToModelUser(cp.User));
            }

            return response;
        }

        public int GetVolunteerCountByPostCode(string postCode)
        {
            return _context.SupportPostcode.Count(x => x.PostalCode == postCode);
        }

        public int GetChampionCountByPostCode(string postCode)
        {
            return _context.ChampionPostcode.Count(x => x.PostalCode == postCode);
        }

        public int PostCreateUser(model.User user)
        {
            var EFuser = MapModelUserToEFUser(user);
            _context.User.Add(EFuser);
            _context.SaveChanges();
            return EFuser.Id;
        }

        private model.User MapEFUserToModelUser(User user)
        {
            return new model.User()
            {
                DateCreated = user.DateCreated,
                ID = user.Id,
                FirebaseUID = user.FirebaseUid,
                EmailSharingConsent = user.EmailSharingConsent,
                HMSContactConsent = user.HmscontactConsent,
                IsVerified = user.IsVerified,
                IsVolunteer = user.IsVolunteer,
                MobileSharingConsent = user.MobileSharingConsent,
                OtherPhoneSharingConsent = user.OtherPhoneSharingConsent,
                PostalCode = user.PostalCode,
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

        public void CreateChampionForPostCode(string userId, string postCode)
        {
            var user = _context.User.Where(a => a.Id.ToString() == userId).FirstOrDefault();

            if(user!=null)
            {
                var result = _context.ChampionPostcode.Where(a => a.User == user && a.PostalCode == postCode).FirstOrDefault();

                if(result==null)
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

        public void CreateSupportForPostCode(string userId, string postCode)
        {
            var user = _context.User.Where(a => a.Id.ToString() == userId).FirstOrDefault();

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
    }
}
