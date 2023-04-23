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
    /// this class represents a database table entity for storing information related to a tcp communication mode.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required parameters for DLMS Scheduler.
    public class TCP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int TCPSchedulerID { get; set; }
        [ForeignKey("TCPSchedulerID")]
        public DLMSScheduler? DLMS { get; set; }
        public string? ClientAddress { get; set; }
        public string? IPAddress { get; set; }
        public string? PortNo { get; set; }
        public string? Profile { get; set; }
        public string? Password { get; set; }


    }
}
