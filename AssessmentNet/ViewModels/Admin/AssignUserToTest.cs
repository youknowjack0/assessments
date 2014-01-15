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
        private Test _test;
        private double _expiryTimeInHours = 48;

        [Required]
        public string UserEmail { get; set; }

        public Test Test
        {
            get { return _test; }
            set
            {
                _test = value;
                TestId = _test.Id;
            }
        }

        public double ExpiryTimeInHours
        {
            get { return _expiryTimeInHours; }
            set { _expiryTimeInHours = value; }
        }

        public int TestId { get; set; }
    }
}