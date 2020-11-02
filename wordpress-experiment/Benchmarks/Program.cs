using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Pchp.Core;
using System;
using System.IO;
using System.Reflection;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<WpBenchmark>();
        }
    }

    [LongRunJob]
    [MemoryDiagnoser]
    public class WpBenchmark
    {
        private static readonly string WordPressProjectDir = Path.GetFullPath(@"../../../../../../../../Website");

        private bool assemblyLoaded = false;

        private readonly MemoryStream binaryOutput = new MemoryStream();
        private readonly StreamWriter textOutput = new StreamWriter(new MemoryStream());

        private void LazyLoadPeachpieAssembly(string configuration)
        {
            if (!this.assemblyLoaded)
            {
                var assembly = Assembly.LoadFrom(@$"{WordPressProjectDir}/bin/{configuration}/netstandard2.0/WordPress.{configuration}.dll");
                Context.AddScriptReference(assembly);
                this.assemblyLoaded = true;
            }
        }

        private void RunBenchmark(string configuration)
        {
            LazyLoadPeachpieAssembly(configuration);

            // Reset streams
            this.binaryOutput.Position = 0;
            this.textOutput.BaseStream.Position = 0;

            // Initialize context
            using var ctx = Context.CreateConsole("index.php");
            ctx.RootPath = ctx.WorkingDirectory = WordPressProjectDir;
            ctx.OutputStream = this.binaryOutput;
            ctx.Output = this.textOutput;
            ctx.Server["HTTP_HOST"] = "localhost";
            ctx.Server["REQUEST_URI"] = "/";

            // Run the script
            var index = Context.TryGetDeclaredScript("index.php");
            try
            {
                index.Evaluate(ctx, PhpArray.NewEmpty(), null);
            }
            catch (ScriptDiedException died)
            {
                died.ProcessStatus(ctx);
            }
        }

        [Benchmark(Baseline = true)]
        public void Release() => RunBenchmark(nameof(Release));

        [Benchmark]
        public void PhpDocForce() => RunBenchmark(nameof(PhpDocForce));

        [Benchmark]
        public void PhpDocOverloadsStatic() => RunBenchmark(nameof(PhpDocOverloadsStatic));

        [Benchmark]
        public void PhpDocOverloadsDynamic() => RunBenchmark(nameof(PhpDocOverloadsDynamic));
    }
}
