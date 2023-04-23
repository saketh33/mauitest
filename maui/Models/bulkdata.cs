using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace DLMSmaui.Models
{
    public class Bulkdata
    {

        [Required]
        public string Jsondata { get; set; }

        public string UserName { get; set; }

    }
}