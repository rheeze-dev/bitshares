using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class EditedDatas
    {

        public int Id { get; set; }

        public DateTime DateEdited { get; set; }

        public string Origin { get; set; }

        public int ControlNumber { get; set; }

        public string EditedBy { get; set; }

        public string EditedData { get; set; }

        public string Remarks { get; set; }

    }
}