using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMSmaui.Models
{
    internal class APIs
    {
        //public const string AuthenticateUser = "/api/Users/AuthenticateUser";
        //public const string RegisterUser = "/api/Users/RegisterUser";
        //public const string RefreshToken = "/api/Users/RefreshToken";

        //public const string RegisterJob = "/Jobs/RegisterJob/RegisterJob";
        //public const string GetAllJobs = "/Jobs/RegisterJob/GetAllJobs";
        //public const string Directschedule = "/Jobs/RegisterJob/Direct/RegisterScheduler";
        //public const string TCPschedule = "/Jobs/RegisterJob/TCP/RegisterScheduler";

        public const string AuthenticateUser = "/api/Users/AuthenticateUser";
        public const string RegisterUser = "/api/Users/RegisterUser";
        public const string RefreshToken = "/api/Users/RefreshToken";



        public const string RegisterJob = "/Jobs/RegisterJob";
        public const string GetAllJobs = "/Jobs/GetAllJobs";
        public const string Directschedule = "/Direct/RegisterScheduler";
        public const string TCPschedule = "/TCP/RegisterScheduler";
        public const string Multischedule = "/Multischedule";
        public const string SerialTCP = "/SerialTCP";
        public const string DLMSSchedulerGetAll = "/DLMS/GetAllSchedules";
    }
}
