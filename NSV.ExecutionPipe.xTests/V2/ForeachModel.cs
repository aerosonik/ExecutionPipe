using System.Collections.Generic;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class ForeachModel
    {
        public int Integer { get; set; }
        public IEnumerable<EnumerableModel> Enumerated { get; set; }
    }

    public class EnumerableModel
    {
        public string Title { get; set; }
        public int Value { get; set; }
        public bool Flag { get; set; }
    }
}
