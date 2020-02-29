using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MxRecords.Models
{
    public class Log
    {
        [Key]
        public int id { get; set; }
        public string session { get; set; }
        public DateTime enteredTime { get; set; }
        public string uploadedFile { get; set; }
    }
}
