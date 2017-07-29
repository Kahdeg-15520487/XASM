using System;
using System.Collections.Generic;
using System.Diagnostics;

using CommandLine;

using XASM;
using XASM.Compiler;
using XASM.VirtualMachine;

namespace testconsole
{
    class Argument
    {
        [Option('o', "output", Required = false)]
        public string binaryname { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool isverbose { get; set; }

        [Value(0, Required = true)]
        public string sourcecode { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Argument>(args)
                .WithParsed(Parsed)
                .WithNotParsed(Errors);
        }

        static void Errors(IEnumerable<CommandLine.Error> errors)
        {
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
        }

        static void Parsed(Argument arg)
        {
            string sourcecode = arg.sourcecode;
            Script script;
            compiler compiler = new compiler();
            script = compiler.Compile(sourcecode, arg.isverbose);
            if (script != null)
            {
                virtualmachine vm = new virtualmachine(isVerbose: arg.isverbose);
                vm.Load(script, new StandardInputOutputHostAPI(Console.In, Console.Out));

                Stopwatch sw = new Stopwatch();
                sw.Start();
                vm.Run();
                sw.Stop();

                Console.WriteLine("script execution take {0} ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
