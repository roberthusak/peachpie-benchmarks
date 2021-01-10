﻿using System;
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

            // Write the compilation results
            var compilationCounters = assembly.GetCustomAttribute<CompilationCountersAttribute>();
            Console.WriteLine("Compilation counters:");
            Console.WriteLine($"Total routines (functions): {compilationCounters.Routines} ({compilationCounters.GlobalFunctions})");
            Console.WriteLine($"Specialized routines: {compilationCounters.Specializations}");
            Console.WriteLine($"Total routine call sites (functions): {compilationCounters.RoutineCalls} ({compilationCounters.FunctionCalls})");
            Console.WriteLine($"Library function call sites (including ambiguous): {compilationCounters.LibraryFunctionCalls}");
            Console.WriteLine($"Ambiguous source function call sites: {compilationCounters.AmbiguousSourceFunctionCalls}");
            Console.WriteLine($"Branched source function call sites: {compilationCounters.BranchedSourceFunctionCalls}");
            Console.WriteLine($"Original/specialized function call sites: {compilationCounters.OriginalSourceFunctionCalls}/{compilationCounters.SpecializedSourceFunctionCalls}");
            Console.WriteLine();

            RunWordPress(wpDir, proofFile);

            // Write the runtime results
            Console.WriteLine("Runtime counters:");
            Console.WriteLine($"Total routine calls (functions): {RuntimeCounters.RoutineCalls} ({RuntimeCounters.GlobalFunctionCalls})");
            Console.WriteLine($"Specialization calls (original/specialized): {RuntimeCounters.OriginalOverloadCalls}/{RuntimeCounters.SpecializedOverloadCalls}");
            Console.WriteLine($"Checks (original/specialized selects): {RuntimeCounters.BranchedCallChecks} ({RuntimeCounters.BranchedCallOriginalSelects}/{RuntimeCounters.BranchedCallSpecializedSelects})");
        }

        private static void RunWordPress(string wpDir, string proofFile)
        {
            // Clean up the output file before the run
            File.WriteAllText(proofFile, "");

            var binOut = new MemoryStream();
            var textOut = new StreamWriter(binOut);
            var ctx = Context.CreateConsole("index.php");
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
        }
    }
}
