using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerTest.Services
{
    public interface ITestService
    {
        Task DoSomething(CancellationToken token);
    }
}
