using System;
using Microsoft.AspNetCore.Http;

namespace SmartGym.Data
{
    public class ProfileDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public IFormFile Photo { get; set; }
    }
}