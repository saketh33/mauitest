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
    /// This class is used to store the constant values used in the application.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.0        03-03-2023          Subiya              Added required constants for DLMS Scheduler.
    public class Constants
    {
       public const string DIRECModeOfCommunication = "direct";
       public const string TCPModeOfCommunication = "TCPIP";
       public const string ScheduleDaily = "Daily";
       public const string ScheduleForDateTime = "Datetime";
       public const string JobRegister = "Jobs/RegisterJob";
       public const string JobGetAll= "Jobs/GetAllJobs";
       public const string DLMSSchedulerGetAll = "DLMS/GetAllSchedules";
       public const string JobGetAllJobName = "Jobs/GetAllJobNames";
       public const string DIRECT = "Direct/RegisterScheduler";
       public const string RS485 = "RS485/RegisterScheduler";
       public const string TCP = "TCP/RegisterScheduler";
       public const string DIRECTGetAll = "SchedulerDetails/GetAllDetailsByJobID";
       public const string BulkUpload = "BulkUpload";
       public const string GetJobByID = "Job/JobID";
       public const string GetScheulerDetailByJobID = "SchedulerDetail/JobID";
       public const string GetScheulerDetailBySchedulerID = "SchedulerDetail/SchedulerID";
       public const string UserController = "api/[controller]";
       public const string CreateRole = "CreateRole";
       public const string AssignRoleToUser = "AssignRoleToUser";
       public const string RefreshToken = "RefreshToken";
       public const string AuthenticateUser = "AuthenticateUser";
       public const string RegisterUser = "RegisterUser";
       public const string DeleteUser = "DeleteUser";
       public const string LogFilePath = "C:\\LogFile\\log.txt";
       public const string Title = "DLMS Scheduler";
       public const string Version = "V1";
       public const string Description = "DLMS Scheduler API is a software interface that allows developers to schedule and monitor meter reading.The API provides a way to set up tasks that can be executed at a specific time or on a recurring basis.The DLMS Scheduler API also provides a mechanism for monitoring the status of tasks and retrieving information about past executions.Overall,the API provides a way to automate and manage complex tasks ";
       public const string SecuritySchemaDescription = "Authorization header using the Bearer scheme. Example \"Authorization: Bearer {JWT_token}\". The JWT token should be a Base64-encoded JSON Web Token containing the required claims.";
       public const string SecuritySchemaName = "Authorization";
       public const string SecuritySchemaScheme = "Bearer";
       public const string ConnectionString = "SchedulerDB";
       public const string DateTimeFormat = "ddMMyyyyHHmmss";
       public const string Multischedule = "Multischedule";
       public const string ScheduleDetails = "SchedulerDetails";
       public const string DeleteJobName = "DeleteJobName";
       public const string UpdateJobName = "UpdateJobName";
       public const string UpdateJobDetail = "UpdateJobDetail";
       public const string DeleteScheduleDetails = "DeleteScheduleDetails";
       public const string JobCount = "JobCount";
        public const string ScheduleCount = "ScheduleCount";






















    }
}
