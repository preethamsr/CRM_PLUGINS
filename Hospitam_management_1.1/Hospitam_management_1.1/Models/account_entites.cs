using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Xrm.Sdk;

namespace Hospitam_management_1._1.Models
{
    public class account_entites
    { 
        [Display(Name ="Patient registration number")]
        public string preetham_registrationnumber { get; set; }
        public string name { get; set; }
        public string Department { get; set; }
        public string Doctor { get; set; }
       [Display(Name ="Reason for registration")]
        public string reason_for_registration { get; set; }
        public Guid account_id { get; set; }
        public Guid department_id { get; set; }
        public Guid doctor_id { get; set; }
        
     
    }
}