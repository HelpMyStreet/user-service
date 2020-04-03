﻿using UserService.Core.Dto;
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

        List<User> GetChampionsByPostCode(string postCode);

        int GetVolunteerCountByPostCode(string postCode);

        int GetChampionCountByPostCode(string postCode);

        int GetChampionPostcodesCoveredCount();

        int GetDistinctChampionUserCount();

        bool GetUserIsVerified(string userId);

        int PostCreateUser(string firebaseUserId, string emailAddress);

        int ModifyUser(User user);

        bool SetUserVerfication(string userId, bool isVerified);

        void CreateChampionForPostCode(string userId, string postCode);

        void CreateSupportForPostCode(string userId, string postCode);
    }
}
