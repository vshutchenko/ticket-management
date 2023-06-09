﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TicketManagement.DataAccess.Entities
{
    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CultureName { get; set; }

        [Column(TypeName = "decimal(16,2)")]
        public decimal Balance { get; set; }

        public string TimeZoneId { get; set; }
    }
}
