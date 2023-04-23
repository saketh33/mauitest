using DLMSmaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMSmaui.Services
{
    public interface IAppService
    {
        Task<bool> RefreshToken();
        public Task<MainResponse> AuthenticateUser(LoginModel loginModel);
        Task<(bool IsSuccess, string ErrorMessage)> RegisterUser(RegistrationModel registerUser);

        Task<(bool IsSuccess, string ErrorMessage)> RegisterJob(JobModel registerjob);

        Task<(bool IsSuccess, string ErrorMessage)> Registersingleschedule(SingleSchedule singleschedule);

        Task<(bool IsSuccess, string ErrorMessage)> Registermultipleschedule(MultipleSchedule mutipleschedule);

        Task<List<FetchJobModel>> GetAllJobs();
        Task<List<Scheduledetails>> GetAllSchdulesByJobID(string jobid);
        public Task<MainResponse> AddJob(AddUpdateJobRequest jobRequest);
        public Task<MainResponse> UpdateJob(AddUpdateJobRequest jobRequest);
        public Task<(bool IsSuccess, string ErrorMessage)> DeleteJob(string jobid);
    }
}
