/*
 * Licensed to the SkyAPM under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The SkyAPM licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SkyApm.Common
{
    internal static class CpuHelpers
    {
        private const int interval = 1000;
        private static double _usagePercent;
        private static readonly Task _task;

        public static double UsagePercent => _usagePercent;

        static CpuHelpers()
        {
            var process = Process.GetCurrentProcess();
            _task = Task.Factory.StartNew(async () =>
            {
                var _prevCpuTime = process.TotalProcessorTime.TotalMilliseconds;
                while (true)
                {
                    var prevCpuTime = _prevCpuTime;
                    var currentCpuTime = process.TotalProcessorTime;
                    var usagePercent = (currentCpuTime.TotalMilliseconds - prevCpuTime) / interval;
                    Interlocked.Exchange(ref _prevCpuTime, currentCpuTime.TotalMilliseconds);
                    Interlocked.Exchange(ref _usagePercent, usagePercent);
                    await Task.Delay(interval);
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}