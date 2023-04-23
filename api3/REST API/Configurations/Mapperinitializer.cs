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

using AutoMapper;
using DLMSScheduler_API;
namespace DLMSScheduler_API
{
    /// <summary>
    /// This class is responsible for initializing AutoMapper mappings between domain entities and their respective DTOs.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required mapping for DLMS Scheduler.


    public class Mapperinitializer:Profile
    {
        public Mapperinitializer()
        {
            // CreateMap<Source, Destination>() - maps Source object to Destination object
            // ReverseMap() - creates a reverse map from Destination object to Source object

            // Maps DLMSScheduler entity to DIRECTSchedulerDTO and vice versa
            CreateMap<DLMSScheduler, DIRECTSchedulerDTO>().ReverseMap();
            // Maps DLMSScheduler entity to TCPSchedulerDTO and vice versa
            CreateMap<DLMSScheduler, TCPSchedulerDTO>().ReverseMap();
            CreateMap<SerialTCP,SerialTCPDTO>().ReverseMap();
            // Maps Job entity to JobDTO and vice versa
            CreateMap<Job, JobDTO>().ReverseMap();



        }
    }
}
