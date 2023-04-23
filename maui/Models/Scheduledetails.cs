using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMSmaui.Models
{
    public class Scheduledetails
    {
        public string SchedulerID { get; set; }
        public string? ModeOfCommunication { get; set; }
        //public string? ModeOfSchedule { get; set; }
        public string? SchedulerRunFrom { get; set; }
        public string? CreatedDate { get; set; }
        public string? MeterStatus { get; set; }
    }
}
