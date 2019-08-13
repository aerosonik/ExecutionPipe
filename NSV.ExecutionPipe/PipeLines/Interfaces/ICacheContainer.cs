using NSV.ExecutionPipe.PipeLines.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSV.ExecutionPipe.PipeLines.Interfaces
{
    public interface ICacheContainer
    {
        IPipeCache Cache { get; set; }
    }
}
