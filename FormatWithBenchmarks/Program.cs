using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;

using FormatWith;
using System.Linq;

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
        public static readonly string Replacement1 = "Replacement1";
        public static readonly string Replacement2 = "Replacement {} Two ";


        public static readonly string TestFormat4 = "abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";
        public static readonly string TestFormat4Composite = "abc{0}def{{escaped1}}ghi{{{1}}}jkl{{{{escaped2}}}}mno";
        public static readonly string TestFormat4Solution = $"abc{Replacement1}def{{escaped1}}ghi{{{Replacement2}}}jkl{{{{escaped2}}}}mno";

        Dictionary<string, string> replacementDictionary = new Dictionary<string, string>()
        {
            ["Replacement1"] = Replacement1,
            ["Replacement2"] = Replacement2
        };

        object replacementObject = new { Replacement1, Replacement2 };

        public FormatWithBenchmarks()
        {
        }

        [Benchmark]
        public void SpeedTestBigger() => TestFormat4.FormatWith(replacementDictionary);

        [Benchmark]
        public void SpeedTestBiggerAnonymous() => TestFormat4.FormatWith(replacementObject);

        [Benchmark]
        public void MultithreadedDict() => Enumerable.Range(1, 1000).AsParallel().ForAll(i => TestFormat4.FormatWith(replacementDictionary));
    }
}
