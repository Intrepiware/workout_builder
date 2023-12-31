﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("User")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(255)]
        public string FirstName { get; set; }

        [Required, MaxLength(255)]
        public string LastName { get; set; }

        [Required, MaxLength(255)]
        public string EmailAddress { get; set; }
        [Required, MaxLength(255)]
        public string Password { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        public DateTime? LockDate { get; set; }
        public DateTime? PasswordResetDate { get; set; }
        public bool IsAdmin { get; set; }
    }
}
