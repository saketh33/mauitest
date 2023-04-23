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
    /// this class represents a database table entity for storing information related to Schdule Timing.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required parameters for DLMS Scheduler.
    public class ScheduleTiming
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ScheduleTimingID { get; set; }
        [ForeignKey("ScheduleTimingID")]

        public DLMSScheduler? DLMS { get; set; }
        public string? RecurrenceType { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public DateTime? Daily { get; set; }
        public string? Recurring { get; set; }
    }
}
