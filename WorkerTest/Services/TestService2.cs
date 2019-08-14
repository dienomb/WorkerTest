using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerTest.Services
{
    public class TestService2 : ITestService
    {
        public async Task DoSomething(CancellationToken token)
        {
            Random random = new Random();
            await Task.Delay(random.Next(50, 1000), token);
        }
    }
}
