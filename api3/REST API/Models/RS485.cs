using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DLMSScheduler_API
{
    public class RS485
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RS485SchedulerID { get; set; }
        [ForeignKey("RS485SchedulerID")]
        public DLMSScheduler? DLMS { get; set; }
        public string? SlaveID { get; set; }
        public string? Parity { get; set; }
        public string? Baudrate { get; set; }
        public string? COMPort { get; set; }
        public string? Profile { get; set; }
        public string? PortNo { get; set; }
        public string? Password { get; set; }

    }
}
