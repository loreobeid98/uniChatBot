using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class Student
    {
        public string name { get; set; }
        public string major { get; set; }
        public int gpa { get; set; }

        public Course C { get; set; }
    }
}
