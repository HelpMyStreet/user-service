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
                PostalCode = user.PostalCode
            };
        }

        public model.User GetUserByID(string userId)
        {
            User user = _context.User.Where(x => x.Id.ToString() == userId).FirstOrDefault();

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
            throw new NotImplementedException();
        }
    }
}
