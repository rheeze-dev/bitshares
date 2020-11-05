using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class MonthlyTimeSheet : BaseEntity
    {
        [Key]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Id Number")]
        public string IdNumber { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public int TotalNumberOfMinWorked { get; set; }

        public int NumberOfMinOT { get; set; }

        public int TotalNumberOfMinTardiness { get; set; }

        [Display(Name = "Editor")]
        public string Editor { get; set; }

        public int ControlNumber { get; set; }

        public string Remarks { get; set; }
    }
}
