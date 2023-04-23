using DLMSmaui.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLMSmaui.Services
{
    public class AppService : IAppService
    {
        public async Task<MainResponse> AuthenticateUser(LoginModel loginModel)
        {
            var returnResponse = new MainResponse();
            using (var client = new HttpClient())
            {
                var url = $"{Setting.BaseUrl}{APIs.AuthenticateUser}";

                var serializedStr = JsonConvert.SerializeObject(loginModel);

                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string contentStr = await response.Content.ReadAsStringAsync();
                    returnResponse = JsonConvert.DeserializeObject<MainResponse>(contentStr);
                }
            }
            return returnResponse;
        }
         
        
        public async Task<bool> RefreshToken()
        {
            bool isTokenRefreshed = false;
            using (var client = new HttpClient())
            {
                var url = $"{Setting.BaseUrl}{APIs.RefreshToken}";

                var serializedStr = JsonConvert.SerializeObject(new AuthenticateRequestAndResponse
                {
                    RefreshToken = Setting.UserBasicDetail.RefreshToken,
                    AccessToken = Setting.UserBasicDetail.AccessToken
                });

                try
                {
                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        string contentStr = await response.Content.ReadAsStringAsync();
                        var mainResponse = JsonConvert.DeserializeObject<MainResponse>(contentStr);
                        if (mainResponse.IsSuccess)
                        {
                            var tokenDetails = JsonConvert.DeserializeObject<AuthenticateRequestAndResponse>(mainResponse.Content.ToString());
                            Setting.UserBasicDetail.AccessToken = tokenDetails.AccessToken;
                            Setting.UserBasicDetail.RefreshToken = tokenDetails.RefreshToken;

                            string userDetailsStr = JsonConvert.SerializeObject(Setting.UserBasicDetail);
                            await SecureStorage.SetAsync(nameof(Setting.UserBasicDetail), userDetailsStr);
                            isTokenRefreshed = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }

                
            }
            return isTokenRefreshed;
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterUser(RegistrationModel registerUser)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            using (var client = new HttpClient())
            {
                var url = $"{Setting.BaseUrl}{APIs.RegisterUser}";

                var serializedStr = JsonConvert.SerializeObject(registerUser);
                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            return (isSuccess, errorMessage);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RegisterJob(JobModel registerjob)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            using (var client = new HttpClient())
            {
                var url = $"{Setting.BaseUrl}{APIs.RegisterJob}";

                var serializedStr = JsonConvert.SerializeObject(registerjob);
                var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    isSuccess = true;
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                }
            }
            return (isSuccess, errorMessage);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> Registersingleschedule(SingleSchedule singleschedule)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            using (var client = new HttpClient())
            {
                
                if (singleschedule.ModeOfCommunication == "Direct")
                {
                    var dataToSend = new {
                        SJobID = singleschedule.SJobID,
                        MeterId = singleschedule.MeterId,
                        ModeOfCommunication = singleschedule.ModeOfCommunication,
                        recurrenceType = singleschedule.ModeOfSchedule,
                        scheduleDate = singleschedule.ScheduleDate,
                        daily=singleschedule.Daily,
                        recurring = singleschedule.Recurring,
                        parity = singleschedule.Parity,
                        baudrate = singleschedule.Baudrate,
                        comPort = singleschedule.COMPort,
                        profile = singleschedule.Profile,
                        password=singleschedule.Password,
                    };
                    var url = $"{Setting.BaseUrl}{APIs.Directschedule}";
                    var serializedStr = JsonConvert.SerializeObject(dataToSend);

                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }

                else if (singleschedule.ModeOfCommunication == "SerialTCP")
                {
                    var dataToSend = new
                    {
                        SJobID = singleschedule.SJobID,
                        MeterId = singleschedule.MeterId,
                        ModeOfCommunication = singleschedule.ModeOfCommunication,
                        recurrenceType = singleschedule.ModeOfSchedule,
                        scheduleDate = singleschedule.ScheduleDate,
                        daily = singleschedule.Daily,
                        recurring = singleschedule.Recurring,
                        clientAddress = singleschedule.SerialTCPcleintid,
                        ipAddress = singleschedule.SerialTCPaddress,
                        portNo = singleschedule.SerialTCPportnumber,
                        profile = singleschedule.Profile,
                        password = singleschedule.Password,
                    };
                    var url = $"{Setting.BaseUrl}{APIs.SerialTCP}";
                    var serializedStr = JsonConvert.SerializeObject(dataToSend);

                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }

                else if (singleschedule.ModeOfCommunication == "TCPIP")
                {
                    var dataToSend = new
                    {
                        SJobID = singleschedule.SJobID,
                        MeterId = singleschedule.MeterId,
                        ModeOfCommunication = singleschedule.ModeOfCommunication,
                        recurrenceType = singleschedule.ModeOfSchedule,
                        scheduleDate = singleschedule.ScheduleDate,
                        daily = singleschedule.Daily,
                        recurring = singleschedule.Recurring,
                        clientAddress = singleschedule.TCPipaddress,
                        ipAddress = singleschedule.TCPclientid,
                        portNo = singleschedule.TCPportnumber,
                        profile = singleschedule.Profile,
                        password = singleschedule.Password,
                    };
                    var url = $"{Setting.BaseUrl}{APIs.TCPschedule}";
                    var serializedStr = JsonConvert.SerializeObject(dataToSend);

                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return (isSuccess, errorMessage);
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> Registermultipleschedule(MultipleSchedule mutipleschedule)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            using (var client = new HttpClient())
            {

                if (mutipleschedule.ModeOfCommunication == "Direct")
                {
                    var dataToSend = new
                    {
                        SJobID = mutipleschedule.SJobName,
                        ModeOfCommunication = mutipleschedule.ModeOfCommunication,
                        Meterdata= mutipleschedule.jsontable,
                        profile = mutipleschedule.Profile,
                        username= mutipleschedule.UserName
                    };
                    var url = $"{Setting.BaseUrl}{APIs.Multischedule}";
                    var serializedStr = JsonConvert.SerializeObject(dataToSend);

                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }

                else if (mutipleschedule.ModeOfCommunication == "SerialTCP")
                {
                    var dataToSend = new
                    {
                        SJobID = mutipleschedule.SJobName,
                        
                        ModeOfCommunication = mutipleschedule.ModeOfCommunication,
                        Meterdata = mutipleschedule.jsontable,
                        profile = mutipleschedule.Profile,
                        username = mutipleschedule.UserName
                    };
                    var url = $"{Setting.BaseUrl}{APIs.Multischedule}";
                    var serializedStr = JsonConvert.SerializeObject(dataToSend);

                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }

                else if (mutipleschedule.ModeOfCommunication == "TCPIP")
                {
                    var dataToSend = new
                    {
                        SJobID = mutipleschedule.SJobName,
                        
                        ModeOfCommunication = mutipleschedule.ModeOfCommunication,
                        Meterdata = mutipleschedule.jsontable,
                        profile = mutipleschedule.Profile,
                        username = mutipleschedule.UserName
                    };
                    var url = $"{Setting.BaseUrl}{APIs.Multischedule}";
                    var serializedStr = JsonConvert.SerializeObject(dataToSend);

                    var response = await client.PostAsync(url, new StringContent(serializedStr, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return (isSuccess, errorMessage);
        }


        public async Task<List<FetchJobModel>> GetAllJobs()
        {
            var returnResponse = new List<FetchJobModel>();
            using (var client = new HttpClient())
            {
                var url = $"{Setting.BaseUrl}{APIs.GetAllJobs}";

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Setting.UserBasicDetail?.AccessToken}");
                var response = await client.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    bool isTokenRefreshed = await RefreshToken();
                    if (isTokenRefreshed) return await GetAllJobs();
                }
                else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string contentStr = await response.Content.ReadAsStringAsync();
                        var mainResponse = JsonConvert.DeserializeObject<MainResponse>(contentStr);
                        if (mainResponse.IsSuccess)
                        {
                            returnResponse = JsonConvert.DeserializeObject<List<FetchJobModel>>(mainResponse.Content.ToString());
                        }
                    }
                }

            }
            return returnResponse;
        }

        public async Task<List<Scheduledetails>>GetAllSchdulesByJobID(string jobid)
        {
            var returnResponse = new List<Scheduledetails>();
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{Setting.BaseUrl}{APIs.DLMSSchedulerGetAll}?JobID={jobid}";
                    var apiResponse = await client.GetAsync(url);


                    if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var response = await apiResponse.Content.ReadAsStringAsync();
                        var deserilizeResponse = JsonConvert.DeserializeObject<MainResponse>(response);

                        if (deserilizeResponse.IsSuccess)
                        {
                            returnResponse = JsonConvert.DeserializeObject<List<Scheduledetails>>(deserilizeResponse.Content.ToString());
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return returnResponse;

        }

        public async Task<(bool IsSuccess, string ErrorMessage)> DeleteJob(string jobid)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Setting.BaseUrl}/DeleteJobName?JobID={jobid}";
                    var request = new HttpRequestMessage();
                    request.Method = HttpMethod.Delete;
                    request.RequestUri = new Uri(url);
                    var apiResponse = await client.SendAsync(request);

                    if (apiResponse.IsSuccessStatusCode)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return (isSuccess, errorMessage);
        }

        public async Task<MainResponse> UpdateJob(AddUpdateJobRequest jobRequest)
        {
            var returnResponse = new MainResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"/api/Students/UpdateStudent";

                    var serializeContent = JsonConvert.SerializeObject(jobRequest);

                    var apiResponse = await client.PutAsync(url, new StringContent(serializeContent, Encoding.UTF8, "application/json"));

                    if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var response = await apiResponse.Content.ReadAsStringAsync();
                        returnResponse = JsonConvert.DeserializeObject<MainResponse>(response);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return returnResponse;
        }

        public async Task<MainResponse> AddJob(AddUpdateJobRequest jobRequest)
        {
            var returnResponse = new MainResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{Setting.BaseUrl}{APIs.RegisterJob}";

                    var serializeContent = JsonConvert.SerializeObject(jobRequest);

                    var apiResponse = await client.PostAsync(url, new StringContent(serializeContent, Encoding.UTF8, "application/json"));

                    if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var response = await apiResponse.Content.ReadAsStringAsync();
                        returnResponse = JsonConvert.DeserializeObject<MainResponse>(response);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return returnResponse;
        }

    }
}
