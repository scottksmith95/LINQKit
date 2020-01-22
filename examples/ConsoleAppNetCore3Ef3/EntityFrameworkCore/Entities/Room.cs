using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleAppNetCore3Ef3.EntityFrameworkCore.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public RoomStatus Status { get; set; }

        public bool AllowedSmoking { get; set; }

        [ForeignKey("RoomDetailId")]
        public RoomDetail RoomDetail { get; set; }

        public int? RoomDetailId { get; set; }

        public Room()
        {
            
        }

        public Room(int number, string name, RoomStatus status, bool allowedSmoking, int roomDetailId)
        {
            Number = number;
            Name = name;
            Status = status;
            AllowedSmoking = allowedSmoking;
            RoomDetailId = roomDetailId;
        }
    }
}