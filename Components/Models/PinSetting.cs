using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayNote.Components.Models
{
    [Table("PinSetting")]
    public class PinSetting
    {
        [PrimaryKey]
        public int Id { get; set; } = 1; // always 1 row

        public string PinHash { get; set; } = "";
        public string PinSalt { get; set; } = "";
    }
}
