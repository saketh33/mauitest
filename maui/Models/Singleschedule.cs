using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DLMSmaui.Models
{
    public class SingleSchedule
    {
        public string SJobID { get; set; }
        public int? MeterId { get; set; }
        public string Password { get; set; }
        public string? ModeOfCommunication { get; set; } = "Direct";
        public string? ModeOfSchedule { get; set; } = "Recurring";

        public DateTime? ScheduleDate { get; set; }
        public DateTime? Daily { get; set; }
        public string? Recurring { get; set; }

        public string? Parity { get; set; }
        public string? Baudrate { get; set; }
        public string? COMPort { get; set; }

        public string? SerialTCPaddress { get; set; }
        public string? SerialTCPcleintid { get; set; }
        public string? SerialTCPportnumber { get; set; }

        public string? TCPipaddress { get; set; }
        public string? TCPclientid { get; set; }
        public string? TCPportnumber { get; set; }

        public string? Profile { get; set; }
        public string UserName { get; set; }
    }
}