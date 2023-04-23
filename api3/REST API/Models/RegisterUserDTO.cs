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
    /// this class represents a data transfer object (DTO) to transfer data related to registered user.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required parameters for DLMS Scheduler.
    public class RegisterUserDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? UserAvatar { get; set; }
    }
}
