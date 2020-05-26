using HelpMyStreet.Utils.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Core.Dto
{
    public class HelperWithinRadiusDTO
    {
        public User User { get; set; }
        public double Distance { get; set; }
    }
}
