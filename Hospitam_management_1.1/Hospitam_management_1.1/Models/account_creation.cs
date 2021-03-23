using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hospitam_management_1._1.Models
{
    public class account_creation
    {
        [Display(Name ="Name")]
        public string name { get; set; }
        
        [Display(Name ="Reason for registration")]
        public string reason_for_registration { get; set; }
        [Display(Name ="Previous Medication")]
        public bool taking_any_medication { get; set; }
        [Display(Name ="Department")]
        public string Department { get; set; }
        [Display(Name ="Doctor")]
        public string Doctor { get; set; }
        public Guid department_id { get; set; }
        public Guid doctor_id { get; set; }
    }

    public class account_creation_1
    {
        public string name { get; set; }
      
        public string preetham_reasonforregistration { get; set; }
        public Guid preetham_DepartmentId { get; set; }
        public Guid preetham_doctorId { get; set; }
    }
    public class deparntment
    {
        public string preetham_name { get; set; }
    }
}