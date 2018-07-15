using BenchmarkDotNet.Running;
using System.Reflection;

namespace SharedProperty.Benchmark.NETCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
        }
    }
}
