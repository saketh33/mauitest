/*
* $Archive: $
* $Revision: $ 1.0
* $Date: $     03-03-2023
* $Author: $   Subiya
*
* 
* All rights reserved.
* 
* * This software is the confidential and proprietary information
* of Schneider Electric.  Copying or reproduction without prior written                                                                                                                                                    
* approval is prohibited.
*/

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DLMSScheduler_API
{
    /// <summary>
    /// this class represents a database table entity for storing information related to Scheduler.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required parameters for DLMS Scheduler.
    public class DLMSScheduler
    {
        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int SchedulerID { get; set; }

        public int? SchedulerJobID { get; set; }


        [ForeignKey("SchedulerJobID")]



        public Job? Job { get; set; }
        public int? MeterId { get; set; }
        public string? MeterStatus { get; set; }

        public string? ModeOfCommunication { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? SchedulerRunFrom { get; set; }
        public DateTime? SchedulerRunTo { get; set; }
        public ICollection<ScheduleTiming>? ScheduleTiming { get; set; }
        public ICollection<TCP>? TCP { get; set; }
        public ICollection<DIRECT>? DIRECT { get; set; }
        public ICollection<SerialTCP>? SerialTCP { get; set; }

        public ICollection<RS485>? RS485 { get; set; }

    }
}
