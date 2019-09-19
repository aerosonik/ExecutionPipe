using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.Models
{
    public struct SemaphoreModel
    {
        public string Name { get; set; }
        public int MaxThread { get; set; }
    }
}
