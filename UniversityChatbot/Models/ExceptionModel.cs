﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class ExceptionModel
    {
        public string id { get; set; }

        public string exceptionType { get; set; }
        public string StackTrace { get; set; }
        public string ClassName { get; set; }
    }
}
