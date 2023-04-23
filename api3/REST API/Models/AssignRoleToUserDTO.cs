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

namespace DLMSScheduler_API
{

    /// <summary>
    /// this class represent a data transfer object (DTO) used for assigning roles to a user .
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required properties for DLMS Scheduler.

    public class AssignRoleToUserDTO
    {
        public string? Email { get; set; }
        public string? RoleName { get; set; }
    }
}
