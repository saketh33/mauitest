using System.ComponentModel.DataAnnotations.Schema;

namespace DLMSScheduler_API
{
    public class RS485SchedulerDTO
    {
        public int? SJobID { get; set; }
        [ForeignKey("SchedulerJobID")]
        public int? MeterId { get; set; }

        public string? ModeOfCommunication { get; set; }
        public string? RecurrenceType { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? Daily { get; set; }
        public string? Recurring { get; set; }

        public string? Parity { get; set; }
        public string? Baudrate { get; set; }
        public string? COMPort { get; set; }
        public string? Profile { get; set; }
        public string? Password { get; set; }
        public string? SlaveID { get; set;}
    }
}
