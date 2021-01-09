using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Pchp.Core;

namespace Stats
{
    class Program
    {
        static void Main(string[] args)
        {
            string configuration = (args.Length == 0) ? "Release" : args[0];
            string solutionDir = Path.GetFullPath("..");
            string wpDir = $"{solutionDir}/Website";
            string proofFile = $"{solutionDir}/proofs/{configuration}.html";

            Console.WriteLine($"Configuration: {configuration}");
            Console.WriteLine();

            var assembly = Assembly.LoadFrom($"{wpDir}/bin/{configuration}/netstandard2.0/WordPress.{configuration}.dll");
            Context.AddScriptReference(assembly);

            // Clean up the output file before the run
            File.WriteAllText(proofFile, "");

            var binOut = new MemoryStream();
            var textOut = new StreamWriter(binOut);

            // Initialize context
            using var ctx = Context.CreateConsole("index.php");
            ctx.RootPath = ctx.WorkingDirectory = wpDir;
            ctx.OutputStream = binOut;
            ctx.Output = textOut;
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

            // Output the result to the file
            textOut.Flush();
            File.WriteAllBytes(proofFile, binOut.ToArray());

            // Write the runtime results
            Console.WriteLine($"Total routine calls (functions): {RuntimeCounters.RoutineCalls} ({RuntimeCounters.GlobalFunctionCalls})");
            Console.WriteLine($"Specialization calls (original/specialized): {RuntimeCounters.OriginalOverloadCalls}/{RuntimeCounters.SpecializedOverloadCalls}");
            Console.WriteLine($"Checks (original/specialized selects): {RuntimeCounters.BranchedCallChecks} ({RuntimeCounters.BranchedCallOriginalSelects}/{RuntimeCounters.BranchedCallSpecializedSelects})");
        }
    }
}
