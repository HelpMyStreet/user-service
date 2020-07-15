using HelpMyStreet.Utils.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using UserService.Repo.EntityFramework.Entities;

namespace UserService.Repo.Helpers
{
    public static class EnumSupportActivityExtensions
    {
        public static void SetEnumSupportActivityData(this EntityTypeBuilder<EnumSupportActivities> entity)
        {
            var activites = Enum.GetValues(typeof(SupportActivities)).Cast<SupportActivities>();

            foreach (var activity in activites)
            {
                entity.HasData(new EnumSupportActivities { Id = (int)activity, Name = activity.ToString() });
            }
        }
    }
}
