using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Models
{
    public class RetryModel
    {
        public int Count { get; set; }
        public int TimeOutMilliseconds { get; set; }
    }
}
