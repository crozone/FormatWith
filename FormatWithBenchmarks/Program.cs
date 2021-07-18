using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;

using FormatWith;
using System.Linq;
using System.Text;

using static FormatWithTests.Shared.TestStrings;

namespace FormatWithBenchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FormatWithBenchmarks>();
        }
    }

    [MemoryDiagnoser]
    public class FormatWithBenchmarks
    {
        [Benchmark]
        public void SpeedTestBigger() => Format4.FormatWith(ReplacementDictionary);

        [Benchmark]
        public void SpeedTestHugeMostlyText() => FormatBigStringMostlyTextInput.FormatWith(ReplacementDictionary);

        [Benchmark]
        public void SpeedTestBiggerAnonymous() => Format4.FormatWith(ReplacementObject);

        [Benchmark]
        public void MultithreadedDict() => Enumerable.Range(1, 1000).AsParallel().ForAll(i => Format4.FormatWith(ReplacementDictionary));
    }
}
