using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NSV.ExecutionPipe.xTests.V2
{
    public class RestrictionTester
    {
        private int _maxCount;

        private int _currentCount;

        private object _lock = new Object();

        public void IncrementCounter()
        {
            lock (_lock)
            {
                _currentCount += 1;
                if (_currentCount > _maxCount)
                    _maxCount = _currentCount;
            }
        }

        public void DecrementCounter()
        {
            lock (_lock)
            {
                if(_currentCount > 0)
                    _currentCount -= 1;
            }
        }

        public int MaxCount
        {
            get { return _maxCount; }
        }

    }
}
