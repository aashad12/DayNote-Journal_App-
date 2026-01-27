using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace DayNote.Models
{
    

    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed(Unique = true)]
        public string EntryDay { get; set; } = ""; // "yyyy-MM-dd"
        public DateTime EntryDate { get; set; }

        [Required]
        public string? Title { get; set; } = "";
        [Required]
        public string Content { get; set; } = "";

        [Required ]
        public string PrimaryMood { get; set; } = "";
        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }
        public string? Tags { get; set; }

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }     
    }
}
