using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AssessmentNet.Models;

namespace AssessmentNet.ViewModels.Admin
{
    public class AssignUserToTest
    {
        [Required]
        public string UserEmail { get; set; }
        public Test Test { get; set; }
    }
}