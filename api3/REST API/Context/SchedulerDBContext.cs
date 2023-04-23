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

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DLMSScheduler_API;
//using DLMSScheduler_API.Models;

namespace DLMSScheduler_API
{

    /// <summary>
    /// This is a  DbContext subclass used for managing database access.
    /// </summary>
    // Modification History:
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      Ver #        Date                Author/Modified By       Remarks
    // ------------------------------------------------------------------------------------------------------------------------------------------------------
    //      1.10        03-03-2023          Subiya              Added required tables for DLMS Scheduler.


    public partial class SchedulerDBContext : IdentityDbContext<Users>
    {
        /// <summary>
        /// This is a constructor for the SchedulerDBContext class.
        /// </summary>
        /// <param name="options"></param>
        public SchedulerDBContext(DbContextOptions<SchedulerDBContext> options)
            : base(options)
        {
        }
        //This is a DbSet property which can be queried and updated in the database. 
       
        public DbSet<DLMSScheduler> DLMSScheduler { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<ScheduleTiming> ScheduleTiming { get; set; }
        public DbSet<TCP> TCP { get; set; }
        public DbSet<DIRECT> DIRECT { get; set; }
        public DbSet<SerialTCP> SerialTCP { get; set; }

        public DbSet<RS485> RS485 { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
