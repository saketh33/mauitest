/*
* $Archive: $
* $Revision: $ 1.0
* $Date: $     03-03-2023
* $Author: $   Subiya
* 
* All rights reserved.
* 
* * This software is the confidential and proprietary information
* of Schneider Electric.  Copying or reproduction without prior written                                                                                                                                                    
* approval is prohibited.
*/


using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Text;


namespace DLMSScheduler_API
{
    /// <summary>
    /// this class provides a foundation for implementing Scheduler management functionality.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.0        03-03-2023          Subiya              Added required methods for DLMS Scheduler.


    //This class is derived from the ControllerBase class, which provides common functionality for HTTP controllers.
    //[Authorize]
    [AllowAnonymous]
    [ApiController]
    public class ApiController : ControllerBase
    {
        //The SchedulerDBContext object is a DbContext that represents a database context in an Entity Framework Core (EF Core) application.
        private readonly SchedulerDBContext _db;
        //The IMapper object is used to map between domain objects and DTOs.
        private readonly IMapper _mapper;
        //The ILogger object is used for logging information, warnings, and errors within the ApiController class
        private readonly ILogger<ApiController> _logger;

        //This defines a constructor for an ApiController class. 
        
        public ApiController(SchedulerDBContext db, IMapper mapper, ILogger<ApiController> logger)
        {
            // Constructor implementation
            _db = db;
            _mapper = mapper;
            _logger = logger;

        }
        /// <summary>
        /// This method is to create a new job.
        /// </summary>
        /// <param name="objJobDTO"></param>
        /// <returns></returns>
        //[Authorize]
       // [AllowAnonymous]
        [HttpPost]
        [Route(Constants.JobRegister)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public async Task<ActionResult<JobDTO>> RegisterJob(JobDTO objJobDTO)

        {
            try
            {
                //Check if user exist in the User table
                bool usernameAlreadyExists = _db.Users.Any(x => x.UserName == objJobDTO.UserName);
                if (usernameAlreadyExists)
                {
                    Job objJob = new Job();
                    objJob.CreatedDate = DateTime.Now;
                    objJob.UpdatedDate = DateTime.Now;
                    objJob.Enabled = objJobDTO.Enabled;
                    objJob.JobName = objJobDTO.JobName;
                    objJob.UserName = objJobDTO.UserName;

                    //The code is using the AddAsync method of a database context to asynchronously add a new entity to the database. 
                    await _db.AddAsync(_mapper.Map<Job>(objJob));
                    // This method asynchronously saves all changes made to the entities
                    await _db.SaveChangesAsync();

                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);

                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to create a new job", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// This method is to display all the registered jobs.
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        [HttpGet]
        [Route(Constants.JobGetAll)] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetJob()
        {
            try
            {

                var response = new MainResponse
                {
                    Content = _db.Job.ToList(),
                    IsSuccess = true,
                    ErrorMessage = ""
                };

                _logger.LogInformation("Job Details Displayed.");
                return Ok(response);
            }
            catch (Exception ex)
            { 
                // Log the exception 
                _logger.LogError("Could not display Job Details", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }



        /// <summary>
        /// This method returns the scheduler details based on the jobid.
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        [HttpGet]
        [Route(Constants.DIRECTGetAll)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetJob(int jobId)
        {
            try
            {
                var job = _db.DLMSScheduler.Where(j => j.SchedulerJobID == jobId).ToList();
                if (job == null)
                {
                    return NotFound();
                }

                var response = new MainResponse
                {
                    Content = job,
                    IsSuccess = true,
                    ErrorMessage = ""
                };

                _logger.LogInformation($"Job Details for Job ID {jobId} Displayed.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError($"Could not display Job Details for Job ID {jobId}", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// This method returns modeofcommunication and ScheduleTiming details based on SchedulerID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       // [AllowAnonymous]
        [HttpGet]
        [Route(Constants.ScheduleDetails)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult SearchId(int id)
        {
            try
            {
                var direct = _db.DIRECT.Join(_db.ScheduleTiming,
                d => d.DIRECTSchedulerID,
                s => s.ScheduleTimingID,
                (d, s) => new { Direct = d, ScheduleTiming = s })
                .Where(j => j.Direct.DIRECTSchedulerID == id && j.ScheduleTiming.ScheduleTimingID == id)
                .ToList();

                if (direct.Count > 0)
                {
                    var response = new MainResponse
                    {
                        Content = direct,
                        IsSuccess = true,
                        ErrorMessage = ""
                    };

                    _logger.LogInformation($"Direct Details for ID {id} Found.");
                    return Ok(response);
                }
                else
                {
                    var rs485 = _db.RS485.Join(_db.ScheduleTiming,
                    r => r.RS485SchedulerID,
                    s => s.ScheduleTimingID,
                    (r, s) => new { RS485 = r, ScheduleTiming = s })
                    .Where(j => j.RS485.RS485SchedulerID == id && j.ScheduleTiming.ScheduleTimingID == id)
                    .ToList();

                    if (rs485.Count > 0)
                    {
                        var response = new MainResponse
                        {
                            Content = rs485,
                            IsSuccess = true,
                            ErrorMessage = ""
                        };

                        _logger.LogInformation($"RS485 Details for ID {id} Found.");
                        return Ok(response);
                    }
                    else
                    {
                        var tcp = _db.TCP.Join(_db.ScheduleTiming,
                        d => d.TCPSchedulerID,
                        s => s.ScheduleTimingID,
                        (d, s) => new { TCP = d, ScheduleTiming = s })
                        .Where(j => j.TCP.TCPSchedulerID == id && j.ScheduleTiming.ScheduleTimingID == id)
                        .ToList();

                        if (tcp.Count > 0)
                        {
                            var response = new MainResponse
                            {
                                Content = tcp,
                                IsSuccess = true,
                                ErrorMessage = ""
                            };

                            _logger.LogInformation($"TCP Details for ID {id} Found.");
                            return Ok(response);
                        }
                        else
                        {
                            var serialtcp = _db.SerialTCP
                            .Join(_db.ScheduleTiming,
                            d => d.SerialTCPSchedulerID,
                            s => s.ScheduleTimingID,
                            (d, s) => new { SerialTCP = d, ScheduleTiming = s })
                            .Where(j => j.SerialTCP.SerialTCPSchedulerID == id && j.ScheduleTiming.ScheduleTimingID == id)
                            .ToList();

                            if (serialtcp.Count > 0)
                            {
                                var response = new MainResponse
                                {
                                    Content = serialtcp,
                                    IsSuccess = true,
                                    ErrorMessage = ""
                                };

                                _logger.LogInformation($"SerialTCP Details for ID {id} Found.");
                                return Ok(response);
                            }
                        }
                    }
                    
                    

                    
                }

                // If the ID is not found in any table, return a not found response
                _logger.LogWarning($"ID {id} Not Found in Any Table.");
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError($"Could not search for ID {id}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }





        /// <summary>
        /// This method is to display all the registered Schedules.
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        [HttpGet]
        [Route(Constants.DLMSSchedulerGetAll)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSchedule(int JobID)
        {
            try
            {

                var response = new MainResponse
                {
                    Content = _db.DLMSScheduler.Where(e => e.SchedulerJobID == JobID).ToList(),
                    IsSuccess = true,
                    ErrorMessage = ""
                };

                _logger.LogInformation("Scheduler Details Displayed.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not display Scheduler Details", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        /// <summary>
        /// This method accepts DIRECTSchedulerDTO object as input and checks for validation and stores in DB.
        /// </summary>
        /// <param name="objdIRECTSchedulerDTO"></param>
        /// <returns></returns>

        //[Authorize]
        [AllowAnonymous]
        [HttpPost]
        [Route(Constants.DIRECT)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddDIRECTSchedulerDetails(DIRECTSchedulerDTO objdIRECTSchedulerDTO)
        {
            try
            {

                //Checks if job exist in job table
                bool jobNameAlreadyExists = _db.Job.Where(x => x.JobID == objdIRECTSchedulerDTO.SJobID).Any();//
                if (jobNameAlreadyExists)
                {
                    //Stores Scheduler details
                    DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                    string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                    //ulong ulongschedulerID = Convert.ToUInt64(strcurrentDateTime);
                    //int intschedulerID = unchecked((int)ulongschedulerID);
                    //intschedulerID = int.MaxValue;
                    int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime));
                    string profile = objdIRECTSchedulerDTO.Profile;
                    profile = profile.Replace("Biling profile", "2");
                    profile = profile.Replace("Daily load profile", "4");
                    profile = profile.Replace("Instantaneous", "1");
                    profile = profile.Replace("Power fail related events", "7");
                    profile = profile.Replace("Voltage related events", "5");
                    profile = profile.Replace("TransactionRelatedEvents", "8");
                    profile = profile.Replace("Current related events", "6");
                    profile = profile.Replace("Other events", "9");
                    profile = profile.Replace("Non-rolled events", "10");
                    profile = profile.Replace("Load Profile", "3");

                    objDLMSScheduler.SchedulerID = intschedulerID;
                    objDLMSScheduler.MeterId = objdIRECTSchedulerDTO.MeterId;
                    objDLMSScheduler.CreatedDate = DateTime.Now;
                    objDLMSScheduler.UpdatedDate = DateTime.Now;
                    objDLMSScheduler.ModeOfCommunication = objdIRECTSchedulerDTO.ModeOfCommunication;
                    objDLMSScheduler.SchedulerJobID = objdIRECTSchedulerDTO.SJobID;

                    //Stores ModeOfCommunication details
                    DIRECT objDIRECT = new DIRECT();
                    objDIRECT.DIRECTSchedulerID = objDLMSScheduler.SchedulerID;
                    objDIRECT.COMPort = objdIRECTSchedulerDTO.COMPort;
                    objDIRECT.Baudrate = objdIRECTSchedulerDTO.Baudrate;
                    objDIRECT.Parity = objdIRECTSchedulerDTO.Parity;
                    objDIRECT.Profile = profile;
                    objDIRECT.Password = objdIRECTSchedulerDTO.Password;
                    await _db.DIRECT.AddAsync(objDIRECT);

                    //Stores Schedule Timing details.
                    ScheduleTiming objScheduleTiming = new ScheduleTiming();
                    objScheduleTiming.RecurrenceType = objdIRECTSchedulerDTO.RecurrenceType;
                    if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                    {
                        objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                        objScheduleTiming.Daily = objdIRECTSchedulerDTO.Daily;
                        await _db.ScheduleTiming.AddAsync(objScheduleTiming);
                    }
                    else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                    {
                        objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                        objScheduleTiming.ScheduleDate = objdIRECTSchedulerDTO.ScheduleDate;
                        await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                    }
                    else
                    {
                        objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                        objScheduleTiming.Recurring = objdIRECTSchedulerDTO.Recurring;
                         await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                    }
                    //The code is using the AddAsync method of a database context to asynchronously add a new entity to the database. 
                    await _db.AddAsync(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                    // This method asynchronously saves all changes made to the entities
                    await _db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);

                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to store data in DB", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        /// <summary>
        /// This method accepts RS485TSchedulerDTO object as input and checks for validation and stores in DB.
        /// </summary>
        /// <param name="objRS485SchedulerDTO"></param>
        /// <returns></returns>

        //[Authorize]
       // [AllowAnonymous]
        [HttpPost]
        [Route(Constants.RS485)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRS485SchedulerDetails(RS485SchedulerDTO objRS485SchedulerDTO)
        {
            try
            {

                //Checks if job exist in job table
                bool jobNameAlreadyExists = _db.Job.Where(x => x.JobID == objRS485SchedulerDTO.SJobID).Any();//
                if (jobNameAlreadyExists)
                {
                    //Stores Scheduler details
                    DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                    string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);

                    int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime));
                    string profile = objRS485SchedulerDTO.Profile;
                    profile = profile.Replace("Biling profile", "2");
                    profile = profile.Replace("Daily load profile", "4");
                    profile = profile.Replace("Instantaneous", "1");
                    profile = profile.Replace("Power fail related events", "7");
                    profile = profile.Replace("Voltage related events", "5");
                    profile = profile.Replace("TransactionRelatedEvents", "8");
                    profile = profile.Replace("Current related events", "6");
                    profile = profile.Replace("Other events", "9");
                    profile = profile.Replace("Non-rolled events", "10");
                    profile = profile.Replace("Load Profile", "3");

                    objDLMSScheduler.SchedulerID = intschedulerID;
                    objDLMSScheduler.MeterId = objRS485SchedulerDTO.MeterId;
                    objDLMSScheduler.CreatedDate = DateTime.Now;
                    objDLMSScheduler.UpdatedDate = DateTime.Now;
                    objDLMSScheduler.ModeOfCommunication = objRS485SchedulerDTO.ModeOfCommunication;
                    objDLMSScheduler.SchedulerJobID = objRS485SchedulerDTO.SJobID;

                    //Stores ModeOfCommunication details
                    RS485 objRS485 = new RS485();
                    objRS485.RS485SchedulerID = objDLMSScheduler.SchedulerID;
                    objRS485.COMPort = objRS485SchedulerDTO.COMPort;
                    objRS485.Baudrate = objRS485SchedulerDTO.Baudrate;
                    objRS485.Parity = objRS485SchedulerDTO.Parity;
                    objRS485.Profile = profile;
                    objRS485.Password = objRS485SchedulerDTO.Password;
                    objRS485.SlaveID= objRS485SchedulerDTO.SlaveID;
                    await _db.RS485.AddAsync(objRS485);

                    //Stores Schedule Timing details.
                    ScheduleTiming objScheduleTiming = new ScheduleTiming();
                    objScheduleTiming.RecurrenceType = objRS485SchedulerDTO.RecurrenceType;
                    if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                    {
                        objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                        objScheduleTiming.Daily = objRS485SchedulerDTO.Daily;
                        await _db.ScheduleTiming.AddAsync(objScheduleTiming);
                    }
                    else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                    {
                        objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                        objScheduleTiming.ScheduleDate = objRS485SchedulerDTO.ScheduleDate;
                        await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                    }
                    else
                    {
                        objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                        objScheduleTiming.Recurring = objRS485SchedulerDTO.Recurring;
                        await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                    }
                    //The code is using the AddAsync method of a database context to asynchronously add a new entity to the database. 
                    await _db.AddAsync(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                    // This method asynchronously saves all changes made to the entities
                    await _db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);

                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to store data in DB", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }











        /// <summary>
        /// This method accepts TCPSchedulerDTO object as input and checks for validation and stores in DB.
        /// </summary>
        /// <param name="tCPSchedulerDTO"></param>
        /// <returns></returns>
        //[Authorize]
       // [AllowAnonymous]
        [HttpPost]
        [Route(Constants.TCP)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TCPSchedulerDTO>> AddTCPSchedulerDetails(TCPSchedulerDTO objtCPSchedulerDTO)
        {
            try
            {
                //Checks if job exist in job table.
                bool jobNameAlreadyExists = _db.Job.Any(x => x.JobID == objtCPSchedulerDTO.SJobID);
                if (jobNameAlreadyExists)
                {


                    //Stores Scheduler details.
                    DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                    string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                    int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime)); ;
                    string profile = objtCPSchedulerDTO.Profile;
                    profile = profile.Replace("Biling profile", "2");
                    profile = profile.Replace("Daily load profile", "4");
                    profile = profile.Replace("Instantaneous", "1");
                    profile = profile.Replace("Power fail related events", "7");
                    profile = profile.Replace("Voltage related events", "5");
                    profile = profile.Replace("TransactionRelatedEvents", "8");
                    profile = profile.Replace("Current related events", "6");
                    profile = profile.Replace("Other events", "9");
                    profile = profile.Replace("Non-rolled events", "10");
                    profile = profile.Replace("Load Profile", "3");

                    objDLMSScheduler.SchedulerID = intschedulerID;
                    objDLMSScheduler.SchedulerJobID = objtCPSchedulerDTO.SJobID;
                    objDLMSScheduler.MeterId = objtCPSchedulerDTO.MeterId;
                    objDLMSScheduler.ModeOfCommunication = objtCPSchedulerDTO.ModeOfCommunication;
                    objDLMSScheduler.CreatedDate = DateTime.Now;
                    objDLMSScheduler.UpdatedDate = DateTime.Now;

                    //Validates IP Address
                    IPAddress? IP;
                    bool isClientAddressValid = IPAddress.TryParse(objtCPSchedulerDTO.ClientAddress, out IP);
                    bool isIPAddressValid = IPAddress.TryParse(objtCPSchedulerDTO.IPAddress, out IP);

                    if (isIPAddressValid && isClientAddressValid)
                    {
                        //Stores ModeOfCommunication details
                        TCP objTCP = new TCP();
                        objTCP.TCPSchedulerID = objDLMSScheduler.SchedulerID;
                        objTCP.PortNo = objtCPSchedulerDTO.PortNo;
                        objTCP.ClientAddress = objtCPSchedulerDTO.IPAddress ;
                        objTCP.IPAddress = objtCPSchedulerDTO.ClientAddress;
                        objTCP.Profile = profile;
                        objTCP.Password = objtCPSchedulerDTO.Password;
                        await _db.TCP.AddAsync(objTCP);

                        //Stores Schedule Timing details.
                        ScheduleTiming objScheduleTiming = new ScheduleTiming();
                        objScheduleTiming.RecurrenceType = objtCPSchedulerDTO.RecurrenceType;
                        if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                        {
                            objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                            objScheduleTiming.Daily = objtCPSchedulerDTO.Daily;
                            await _db.ScheduleTiming.AddAsync(objScheduleTiming);
                        }
                        else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                        {
                            objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                            objScheduleTiming.ScheduleDate = objtCPSchedulerDTO.ScheduleDate;
                            await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                        }
                        else
                        {
                            objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                            objScheduleTiming.Recurring = objtCPSchedulerDTO.Recurring;
                            await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                        }

                    }
                    //The code is using the AddAsync method of a database context to asynchronously add a new entity to the database. 
                    await _db.AddAsync(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                    // This method asynchronously saves all changes made to the entities
                    await _db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);

                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to store data in DB", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }






        /// <summary>
        /// This method accepts TCPSchedulerDTO object as input and checks for validation and stores in DB.
        /// </summary>
        /// <param name="serialTCPDTO"></param>
        /// <returns></returns>
       // [Authorize]
       // [AllowAnonymous]
        [HttpPost]
        [Route("SerialTCP")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<TCPSchedulerDTO>> AddSerialTCPSchedulerDetails(SerialTCPDTO serialTCPDTO)
        {
            try
            {
                //Checks if job exist in job table.
                bool jobNameAlreadyExists = _db.Job.Any(x => x.JobID == serialTCPDTO.SJobID);
                if (jobNameAlreadyExists)
                {


                    //Stores Scheduler details.
                    DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                    string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                    int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime)); ;
                    string profile = serialTCPDTO.Profile;
                    profile = profile.Replace("Biling profile", "2");
                    profile = profile.Replace("Daily load profile", "4");
                    profile = profile.Replace("Instantaneous", "1");
                    profile = profile.Replace("Power fail related events", "7");
                    profile = profile.Replace("Voltage related events", "5");
                    profile = profile.Replace("TransactionRelatedEvents", "8");
                    profile = profile.Replace("Current related events", "6");
                    profile = profile.Replace("Other events", "9");
                    profile = profile.Replace("Non-rolled events", "10");
                    profile = profile.Replace("Load Profile", "3");

                    objDLMSScheduler.SchedulerID = intschedulerID;
                    objDLMSScheduler.SchedulerJobID = serialTCPDTO.SJobID;
                    objDLMSScheduler.MeterId = serialTCPDTO.MeterId;
                    objDLMSScheduler.ModeOfCommunication = serialTCPDTO.ModeOfCommunication;
                    objDLMSScheduler.CreatedDate = DateTime.Now;
                    objDLMSScheduler.UpdatedDate = DateTime.Now;

                    //Validates IP Address
                    IPAddress? IP;
                    bool isClientAddressValid = IPAddress.TryParse(serialTCPDTO.ClientAddress, out IP);
                    bool isIPAddressValid = IPAddress.TryParse(serialTCPDTO.IPAddress, out IP);

                    if (isIPAddressValid && isClientAddressValid)
                    {
                        //Stores ModeOfCommunication details
                        SerialTCP objSerialTCP = new SerialTCP();
                        objSerialTCP.SerialTCPSchedulerID = objDLMSScheduler.SchedulerID;
                        objSerialTCP.PortNo = serialTCPDTO.PortNo;
                        objSerialTCP.ClientAddress = serialTCPDTO.ClientAddress;
                        objSerialTCP.IPAddress = serialTCPDTO.IPAddress;
                        objSerialTCP.Profile = profile;
                        objSerialTCP.Password = serialTCPDTO.Password;
                        await _db.SerialTCP.AddAsync(objSerialTCP);

                        //Stores Schedule Timing details.
                        ScheduleTiming objScheduleTiming = new ScheduleTiming();
                        objScheduleTiming.RecurrenceType = serialTCPDTO.RecurrenceType;
                        if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                        {
                            objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                            objScheduleTiming.Daily = serialTCPDTO.Daily;
                            await _db.ScheduleTiming.AddAsync(objScheduleTiming);
                        }
                        else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                        {
                            objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                            objScheduleTiming.ScheduleDate = serialTCPDTO.ScheduleDate;
                            await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                        }
                        else
                        {
                            objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                            objScheduleTiming.Recurring = serialTCPDTO.Recurring;
                            await _db.ScheduleTiming.AddAsync(objScheduleTiming);

                        }

                    }
                    //The code is using the AddAsync method of a database context to asynchronously add a new entity to the database. 
                    await _db.AddAsync(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                    // This method asynchronously saves all changes made to the entities
                    await _db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);

                }
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Unable to store data in DB", ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }









      




        /// <summary>
        /// This method allows you to retrieve data from the job table based on the JobID
        /// </summary>
        /// <param name="JobName"></param>
        /// <returns></returns>
        // [Authorize]
        //[AllowAnonymous]
        [HttpGet(Constants.GetJobByID)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Job>> GetJobDetailsByJobID(int JobID)
        {
            try
            {
                // Retrieve the data from the database based on the JobName
                var data = await _db.Job.Where(e => e.JobID == JobID).ToListAsync();


                if (data == null)
                {
                    // Return a 404 Not Found response if the data is not found
                    return NotFound();
                }

                // Return the data with a 200 OK response
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching data from the database.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

      
        
        /// <summary>
        /// This method allows us to fetch scheduler detail by Job ID.
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns></returns>

       // [Authorize]
        //[HttpGet(Constants.GetScheulerDetailByJobID)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetSchedulerDetailByJobID(int JobID)
        //{
            
        //     try
        //     {
        //        // Retrieve the data from the database 
        //        var jobName =  await _db.Job
        //                     .Where(e => e.JobID == JobID)
        //                     .Select(e => e.JobName)
        //                     .FirstOrDefaultAsync();

        //        var data =await _db.DLMSScheduler.Where(e => e.SchedulerJobID == jobName).ToListAsync();

        //                if (data == null)
        //                {
        //                    // Return a 404 Not Found response if the data is not found
        //                    return NotFound();
        //                }

        //                // Return the data with a 200 OK response
        //                return Ok(data);
        //     }
        //     catch (Exception ex)
        //     {
        //                // Log the exception
        //                _logger.LogError(ex, "An error occurred while fetching data from the database.");

        //                // Return a 500 Internal Server Error response
        //                return StatusCode(StatusCodes.Status500InternalServerError);
        //     }
                
               
        //}

        /// <summary>
        /// This method allows us to fetch data from the DLMSScheduler table by it Scheduler ID.
        /// </summary>
        /// <param name="SchedulerID"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet(Constants.GetScheulerDetailBySchedulerID)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSchedulerDetailBySchedulerID(int SchedulerID)
        {

            try
            {
                // Retrieve the data from the database based on the JobName
                var data = await _db.DLMSScheduler.Where(e => e.SchedulerID == SchedulerID).ToListAsync();

                if (data == null)
                {
                    // Return a 404 Not Found response if the data is not found
                    return NotFound();
                }

                // Return the data with a 200 OK response
                return Ok(data);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching data from the database.");

                // Return a 500 Internal Server Error response
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }






        public static readonly object _lock = new object();
        /// <summary>
        /// This method allows us to deserialze a JSON String and store the values in their appropriate columns.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route(Constants.Multischedule)]

        public async Task<IActionResult> MultipleSchedule()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                try
                {
                    string content = await reader.ReadToEndAsync();

                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                    int sJobID = int.Parse(dict["SJobID"]);
                    string modeOfCommunication = dict["ModeOfCommunication"];
                    string profile = dict["profile"];
                    profile = profile.Replace("Biling profile", "2");
                    profile = profile.Replace("Daily load profile", "4");
                    profile = profile.Replace("Instantaneous", "1");
                    profile = profile.Replace("Power fail related events", "7");
                    profile = profile.Replace("Voltage related events", "5");
                    profile = profile.Replace("TransactionRelatedEvents", "8");
                    profile = profile.Replace("Current related events", "6");
                    profile = profile.Replace("Other events", "9");
                    profile = profile.Replace("Non-rolled events", "10");
                    profile = profile.Replace("Load Profile", "3");
                    List<Dictionary<string, string>> table = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(dict["Meterdata"].ToString());
                    if (modeOfCommunication == "Direct")
                    {
                        foreach (var row in table)
                        {
                            DIRECT objDIRECT = new DIRECT();
                            //TCP objTCP = new TCP();
                            ScheduleTiming objScheduleTiming = new ScheduleTiming();
                            DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                            //BulkUpload? schedulerdetail = objBulk.Schedulerdetail;

                            string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                            //ulong ulongschedulerID = Convert.ToUInt64(strcurrentDateTime);
                            //int intschedulerID = unchecked((int)ulongschedulerID);
                            //intschedulerID = int.MaxValue;


                            int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime)); 
                            objDLMSScheduler.SchedulerID = intschedulerID;
                            //objDLMSScheduler.MeterId = Schedulerdetail.MeterId;
                            objDLMSScheduler.CreatedDate = DateTime.Now;
                            objDLMSScheduler.UpdatedDate = DateTime.Now;
                            objDLMSScheduler.ModeOfCommunication = modeOfCommunication;
                            objDLMSScheduler.SchedulerJobID = sJobID;

                            objDIRECT.DIRECTSchedulerID = objDLMSScheduler.SchedulerID;
                            objDIRECT.COMPort = row["COM Port"];
                            objDIRECT.Baudrate = row["Baud Rate"];
                            objDIRECT.Parity = row["Parity"];
                            objDIRECT.Password = row["Password"];
                            objDIRECT.Profile = profile;

                            objScheduleTiming.RecurrenceType = row["Schedule Type"];
                            if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                            {
                                objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                objScheduleTiming.Daily = DateTime.Parse(row["Schedule Time"]);

                                //objScheduleTiming.Daily = row["Schedule Time"];
                            }
                            else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                            {
                                objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                objScheduleTiming.ScheduleDate = DateTime.Parse(row["Schedule Time"]);
                            }
                            else
                            {
                                objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                objScheduleTiming.Recurring = row["Schedule Time"];
                            }
                            lock (_lock)
                            {
                                _db.DIRECT.Add(objDIRECT);
                                _db.ScheduleTiming.Add(objScheduleTiming);
                                _db.Add(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                                _db.SaveChanges();
                            }
                            await Task.Delay(1000);



                        }
                    }

                    else if (modeOfCommunication == "RS485")
                    {
                        foreach (var row in table)
                        {
                            RS485 objRS485 = new RS485();
                            //TCP objTCP = new TCP();
                            ScheduleTiming objScheduleTiming = new ScheduleTiming();
                            DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                            //BulkUpload? schedulerdetail = objBulk.Schedulerdetail;

                            string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                            int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime)); ;
                            objDLMSScheduler.SchedulerID = intschedulerID;
                            //objDLMSScheduler.MeterId = Schedulerdetail.MeterId;
                            objDLMSScheduler.CreatedDate = DateTime.Now;
                            objDLMSScheduler.UpdatedDate = DateTime.Now;
                            objDLMSScheduler.ModeOfCommunication = modeOfCommunication;
                            objDLMSScheduler.SchedulerJobID = sJobID;

                            objRS485.RS485SchedulerID = objDLMSScheduler.SchedulerID;
                            objRS485.COMPort = row["COM Port"];
                            objRS485.Baudrate = row["Baud Rate"];
                            objRS485.Parity = row["Parity"];
                            objRS485.Password = row["Password"];
                            objRS485.Profile = profile;
                            objRS485.SlaveID = row["Slave ID"];

                            objScheduleTiming.RecurrenceType = row["Schedule Type"];
                            if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                            {
                                objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                objScheduleTiming.Daily = DateTime.Parse(row["Schedule Time"]);

                                //objScheduleTiming.Daily = row["Schedule Time"];
                            }
                            else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                            {
                                objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                objScheduleTiming.ScheduleDate = DateTime.Parse(row["Schedule Time"]);
                            }
                            else
                            {
                                objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                objScheduleTiming.Recurring = row["Schedule Time"];
                            }
                            lock (_lock)
                            {
                                _db.RS485.Add(objRS485);
                                _db.ScheduleTiming.Add(objScheduleTiming);
                                _db.Add(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                                _db.SaveChanges();
                            }
                            await Task.Delay(1000);



                        }
                    }


                    else if(modeOfCommunication=="TCP IP")
                    {
                        foreach (var row in table)
                        {
                            //DIRECT objDIRECT = new DIRECT();
                            TCP objTCP = new TCP();
                            ScheduleTiming objScheduleTiming = new ScheduleTiming();
                            DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                            //BulkUpload? schedulerdetail = objBulk.Schedulerdetail;

                            string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                            int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime)); ;
                            objDLMSScheduler.SchedulerID = intschedulerID;
                            //objDLMSScheduler.MeterId = Schedulerdetail.MeterId;
                            objDLMSScheduler.CreatedDate = DateTime.Now;
                            objDLMSScheduler.UpdatedDate = DateTime.Now;
                            objDLMSScheduler.ModeOfCommunication = modeOfCommunication;
                            objDLMSScheduler.SchedulerJobID = sJobID;

                            string strClientAddress = row["Client IP"];
                            string strIPAddress = row["Server IP"];


                            //Validates the IP Address.
                            IPAddress? IP;
                            bool isClientAddressValid = IPAddress.TryParse(strClientAddress, out IP);
                            bool isIPAddressValid = IPAddress.TryParse(strIPAddress, out IP);

                            if (isIPAddressValid && isClientAddressValid)
                            {
                                objTCP.TCPSchedulerID = objDLMSScheduler.SchedulerID;
                                objTCP.PortNo = row["Server Port"];
                                objTCP.ClientAddress = strClientAddress;
                                objTCP.IPAddress = strIPAddress;
                                objTCP.Profile = profile;
                                objTCP.Password= row["Password"];

                                objScheduleTiming.RecurrenceType = row["Schedule Type"];
                                if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                                {
                                    objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                    objScheduleTiming.Daily = DateTime.Parse(row["Schedule Time"]);

                                    //objScheduleTiming.Daily = row["Schedule Time"];
                                }
                                else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                                {
                                    objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                    objScheduleTiming.ScheduleDate = DateTime.Parse(row["Schedule Time"]);
                                }
                                else
                                {
                                    objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                    objScheduleTiming.Recurring = row["Schedule Time"];
                                }
                                lock (_lock)
                                {
                                    _db.TCP.Add(objTCP);
                                    _db.ScheduleTiming.Add(objScheduleTiming);
                                    _db.Add(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                                    _db.SaveChanges();
                                }
                                await Task.Delay(1000);
                            }



                        }

                    }
                    else if (modeOfCommunication == "SerialTCP")
                    {
                        foreach (var row in table)
                        {
                            //DIRECT objDIRECT = new DIRECT();
                            SerialTCP objSerialTCP= new SerialTCP();
                            ScheduleTiming objScheduleTiming = new ScheduleTiming();
                            DLMSScheduler objDLMSScheduler = new DLMSScheduler();
                            //BulkUpload? schedulerdetail = objBulk.Schedulerdetail;

                            string strcurrentDateTime = DateTime.Now.ToString(Constants.DateTimeFormat);
                            int intschedulerID = Math.Abs((int)Convert.ToInt64(strcurrentDateTime)); ;
                            objDLMSScheduler.SchedulerID = intschedulerID;
                            //objDLMSScheduler.MeterId = Schedulerdetail.MeterId;
                            objDLMSScheduler.CreatedDate = DateTime.Now;
                            objDLMSScheduler.UpdatedDate = DateTime.Now;
                            objDLMSScheduler.ModeOfCommunication = modeOfCommunication;
                            objDLMSScheduler.SchedulerJobID = sJobID;

                            string strClientAddress = row["Server IP"];
                            string strIPAddress = row["Server IP"];


                            //Validates the IP Address.
                            IPAddress? IP;
                            bool isClientAddressValid = IPAddress.TryParse(strClientAddress, out IP);
                            bool isIPAddressValid = IPAddress.TryParse(strIPAddress, out IP);

                            if (isIPAddressValid && isClientAddressValid)
                            {

                                objSerialTCP.SerialTCPSchedulerID = objDLMSScheduler.SchedulerID;
                                objSerialTCP.PortNo = row["Server Port"];
                               objSerialTCP.ClientAddress = strClientAddress;
                               objSerialTCP.IPAddress = strIPAddress;
                                objSerialTCP.Profile = profile;
                                objSerialTCP.Password = row["Password"];

                                objScheduleTiming.RecurrenceType = row["Schedule Type"];
                                if (objScheduleTiming.RecurrenceType == Constants.ScheduleDaily)
                                {
                                    objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                    objScheduleTiming.Daily = DateTime.Parse(row["Schedule Time"]);

                                    //objScheduleTiming.Daily = row["Schedule Time"];
                                }
                                else if (objScheduleTiming.RecurrenceType == Constants.ScheduleForDateTime)
                                {
                                    objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                    objScheduleTiming.ScheduleDate = DateTime.Parse(row["Schedule Time"]);
                                }
                                else
                                {
                                    objScheduleTiming.ScheduleTimingID = objDLMSScheduler.SchedulerID;
                                    objScheduleTiming.Recurring = row["Schedule Time"];
                                }
                                lock (_lock)
                                {
                                    _db.SerialTCP.Add(objSerialTCP);
                                    _db.ScheduleTiming.Add(objScheduleTiming);
                                    _db.Add(_mapper.Map<DLMSScheduler>(objDLMSScheduler));
                                    _db.SaveChanges();
                                }
                                await Task.Delay(1000);
                            }



                        }

                    }

                    return StatusCode(StatusCodes.Status201Created);

                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to store data in DB", ex);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }




      



        /// <summary>
        /// This method deletes the Job detail based on the job id
        /// </summary>
        /// <param name="JobID"></param>
        /// <returns></returns>
        [HttpDelete(Constants.DeleteJobName)]
        [AllowAnonymous]
        public IActionResult DeleteJob(int JobID)
        {
            try
            {
                // Find the schedule record to delete
                var job = _db.Job.SingleOrDefault(e => e.JobID == JobID);
                var schedule=_db.DLMSScheduler.SingleOrDefault(j => j.SchedulerJobID== JobID);

                if ( schedule== null && job==null)
                {
                    // If the record is not found, return a 404 Not Found response
                    return NotFound();
                }
                else if(job ==null)
                {
                    return NotFound();
                }
                else if(schedule!=null)
                {
                    return BadRequest("Delete Existing Schedules");

                }


                // Remove the schedule record from the database
                _db.Job.Remove(job);
                _db.SaveChanges();

                // Return a success response with an empty content
                return Ok(new MainResponse
                {
                    IsSuccess = true,
                });
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not delete Scheduler Details", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// This method updates the Job Name in the job table
        /// </summary>
        /// <param name="JobID"></param>
        /// <param name="JobName"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost(Constants.UpdateJobName)]
        public IActionResult UpdateJobName(int JobID, string JobName)
        {
            try
            {
                // Find the schedule record to update
                var job = _db.Job.SingleOrDefault(e => e.JobID == JobID);

                if (job == null)
                {
                    // If the record is not found, return a 404 Not Found response
                    return NotFound();
                }

                // Update the job fields with the new values
                job.JobName = JobName;
                //job.Description = updatedJob.Description;
                //job.IsEnabled = updatedJob.IsEnabled;

                // Save changes to the database
                _db.SaveChanges();

                // Return the updated job object
                return Ok(new MainResponse
                {
                    IsSuccess = true,
                    Content=job,
                }); 
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not update Job Details", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        /// <summary>
        /// This method updates the Job Details
        /// </summary>
        /// <param name="JobID"></param>
        /// <param name="JobEnabled"></param>
        /// <returns></returns>
        [HttpPost(Constants.UpdateJobDetail)]
        [AllowAnonymous]
        public IActionResult UpdateJobDetail(int JobID, string JobEnabled)
        {
            try
            {
                // Find the schedule record to update
                var job = _db.Job.SingleOrDefault(e => e.JobID == JobID);

                if (job == null)
                {
                    // If the record is not found, return a 404 Not Found response
                    return NotFound();
                }

                // Update the job fields with the new values
                job.Enabled = JobEnabled;
                //job.Description = updatedJob.Description;
                //job.IsEnabled = updatedJob.IsEnabled;

                // Save changes to the database
                _db.SaveChanges();

                // Return the updated job object
                return Ok(new MainResponse
                {
                    IsSuccess = true,
                    Content = job,
                }); 
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not update Job Details", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete(Constants.DeleteScheduleDetails)]
        [AllowAnonymous]
        //[Authorize(Roles = "admin")]

        public IActionResult DeleteSchedulerDetails(int JobID)
        {
            try
            {
                // Find the schedule record to delete
                var job = _db.Job.SingleOrDefault(e => e.JobID == JobID);
                var schedule = _db.DLMSScheduler.SingleOrDefault(j => j.SchedulerJobID == JobID);

                if (schedule == null && job == null)
                {
                    // If the record is not found, return a 404 Not Found response
                    return NotFound();
                }
                else if (job == null)
                {
                    return NotFound();
                }
              


                // Remove the schedule record from the database
                _db.DLMSScheduler.Remove(schedule);
                _db.SaveChanges();

                // Return a success response with an empty content
                return Ok(new MainResponse
                {
                    IsSuccess = true,
                });
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not delete Scheduler Details", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        ///DASHBOARD CALLS/////////

        [HttpGet]
        [Route(Constants.JobCount)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetJobCount()
        {
            try
            {
                int count = _db.Job.Count();


                    var response = new MainResponse
                    {
                        Content = count,
                        IsSuccess = true,
                        ErrorMessage = ""
                    };

                _logger.LogInformation("Job Count Displayed.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not display Job Count", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet]
        [Route(Constants.ScheduleCount)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetScheduleCount()
        {
            try
            {
                int count = _db.DLMSScheduler.Count();


                var response = new MainResponse
                {
                    Content = count,
                    IsSuccess = true,
                    ErrorMessage = ""
                };

                _logger.LogInformation("Job Count Displayed.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception 
                _logger.LogError("Could not display Job Count", ex);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}






    

