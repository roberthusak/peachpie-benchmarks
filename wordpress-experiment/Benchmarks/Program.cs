using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Pchp.Core;
using System;
using System.IO;

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
        private MemoryStream BinaryOutput = new MemoryStream();
        private StreamWriter TextOutput = new StreamWriter(new MemoryStream());

        private Context InitContext()
        {
            Context.AddScriptReference(typeof(WP).Assembly);

            BinaryOutput.Position = 0;
            TextOutput.BaseStream.Position = 0;

            using var ctx = Context.CreateConsole("index.php");
            ctx.RootPath = ctx.WorkingDirectory = Path.GetFullPath(@"..\..\..\..\Website");
            ctx.OutputStream = BinaryOutput;
            ctx.Output = TextOutput;
            ctx.Server["HTTP_HOST"] = "localhost";
            ctx.Server["REQUEST_URI"] = "/";

            return ctx;
        }

        [Benchmark(Baseline = true)]
        public void Default()
        {
            var ctx = InitContext();
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
    }
}
