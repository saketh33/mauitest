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
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DLMSScheduler_API
{
    /// <summary>
    /// This class represents a user entity, with properties for their personal information and authentication tokens.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required properties for DLMS Scheduler.

    //This class inherits from the IdentityUser class.
    public class Users : IdentityUser
    {
        [MaxLength(50)]
        public string? FirstName { get; set; } = null!;

        [MaxLength(50)]
        public string? LastName { get; set; } = null!;
     

        [MaxLength(6)]
        public string? Gender { get; set; } = null!;
        public string? Address { get; set; }
        public string? RefreshToken { get; set; }
        public string? UserAvatar { get; set; }

    }
}
