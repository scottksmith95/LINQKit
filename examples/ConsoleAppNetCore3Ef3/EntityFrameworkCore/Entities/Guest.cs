using System;
using System.ComponentModel.DataAnnotations;

namespace ConsoleAppNetCore3Ef3.EntityFrameworkCore.Entities
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; }

        public DateTime RegisterDate { get; set; }

        public int? NullableInt { get; set; }

        public Guest()
        {
            
        }

        public Guest(string name, DateTime registerDate)
        {
            Name = name;
            RegisterDate = registerDate;
        }
    }
}