using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DLMSmaui.Models
{
    public class MultipleSchedule
    {
        public string SJobName { get; set; }
        public string? ModeOfCommunication { get; set; } = "Direct";

        public string? jsontable { get; set; }

        public string? Profile { get; set; }
        public string UserName { get; set; }             
    }
}