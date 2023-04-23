using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DLMSScheduler_API
{
    public class SerialTCP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int SerialTCPSchedulerID { get; set; }
        [ForeignKey("SerialTCPSchedulerID")]
        public DLMSScheduler? DLMS { get; set; }
        public string? ClientAddress { get; set; }
        public string? IPAddress { get; set; }
        public string? PortNo { get; set; }
        public string? Profile { get; set; }
        public string? Password { get; set; }
    }
}
