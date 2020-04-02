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
        User GetUserByID(string userId);

        List<User> GetVolunteersByPostCode(string postCode);

        List<User> GetChampionsByPostCode(string postCode);

        int GetVolunteerCountByPostCode(string postCode);

        int GetChampionCountByPostCode(string postCode);

        bool GetUserIsVerified(string userId);

        int PostCreateUser(User user);
    }
}
