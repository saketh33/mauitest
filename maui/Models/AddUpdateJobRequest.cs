using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace DLMSmaui.Models
{
    public class AddUpdateJobRequest
    {
        public string JobID { get; set; }
        public string JobName { get; set; }
        public string UserName { get; set; }
    }
}