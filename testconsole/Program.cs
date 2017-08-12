using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

using CommandLine;

using XASM;
using XASM.Compiler;
using XASM.VirtualMachine;


namespace testconsole
{
    [Verb("compile",HelpText = "Compile and run a XASM source file")]
    class CompileOption
    {
        [Option('s', "save", Required = false)]
        public string binaryname { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool isverbose { get; set; }

        [Option('r',"run",Required =false)]
        public bool isrun { get; set; }

        [Value(0, Required = true)]
        public string sourcecode { get; set; }
    }

    [Verb("run",HelpText ="Run a XSE binary file")]
    class RunOption
    {
        [Option('f', "function", HelpText = "function's name to run default to main", Required = false)]
        public string funcname { get; set; }

        [Option('v', "verbose", Required = false)]
        public bool isverbose { get; set; }

        [Value(0,Required =true)]
        public string binaryname { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<CompileOption,RunOption>(args)
                .WithParsed<CompileOption>(CompileParsed)
                .WithParsed<RunOption>(RunParsed)
                .WithNotParsed(Errors);
        }

        static void Errors(IEnumerable<CommandLine.Error> errors)
        {
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
        }

        static void RunParsed(RunOption arg)
        {
            string binaryname = arg.binaryname;
            Script script = Load(binaryname);
            if (string.IsNullOrEmpty(arg.funcname))
            {
                Run(script, arg.isverbose);
            }
            else
            {
                Run(script, arg.isverbose,arg.funcname);
            }
        }

        static void CompileParsed(CompileOption arg)
        {
            //source file path
            string sourcecode = arg.sourcecode;

            compiler compiler = new compiler();
            Script script = compiler.Compile(sourcecode, arg.isverbose, new StandardInputOutputHostAPI(Console.In, Console.Out), new NumberHostAPI());
            
            if (script == null)
            {
                Console.WriteLine("Compilation failed!");
                return;
            }

            if (!string.IsNullOrEmpty(arg.binaryname))
            {
                Save(script, arg.binaryname);
            }

            if (arg.isrun)
            {
                if (script.mainFuncIndex != -1)
                {
                    Run(script, arg.isverbose);
                }
                else
                {
                    Console.WriteLine("There is no main function in the script!");
                    Console.WriteLine("Please enter function's name to execute: ");
                    string funcname = Console.ReadLine();
                    Run(script, arg.isverbose, funcname);
                }
            }
        }

        static void Run(Script script, bool isVerbose, string funcname = "main")
        {
            List<Value> parameters = new List<Value>();

            Function func = script.functiontable.FirstOrDefault(f =>
                {
                    return string.Compare(f.funcName, funcname, true) == 0;
                });

            if (func == null)
            {
                Console.WriteLine("there is no such function!");
                return;
            }

            if (func.paramCount > 0)
            {
                Console.WriteLine("this function have " + func.paramCount + " parameters");
                for (int i = 0; i < func.paramCount; i++)
                {
                    Console.WriteLine("enter parameter " + i + ": ");
                    string tempValue = Console.ReadLine();
                    int tempInt;
                    float tempFloat;
                    char tempChar;
                    if (int.TryParse(tempValue, out tempInt))
                    {
                        parameters.Add(new Value(tempInt));
                    }
                    else
                    {
                        if (float.TryParse(tempValue, out tempFloat))
                        {
                            parameters.Add(new Value(tempFloat));
                        }
                        else
                        {
                            if (char.TryParse(tempValue, out tempChar))
                            {
                                parameters.Add(new Value(tempChar));
                            }
                            else
                            {
                                parameters.Add(new Value(tempValue));
                            }
                        }
                    }
                }
            }


            virtualmachine vm = new virtualmachine(Console.In, Console.Out, isVerbose: isVerbose);
            vm.Load(script, new StandardInputOutputHostAPI(Console.In, Console.Out), new NumberHostAPI());

            Stopwatch sw = new Stopwatch();
            sw.Start();
            vm.Run(funcname, parameters.ToArray<Value>());
            sw.Stop();

            Console.WriteLine("script execution take {0} ms", sw.ElapsedMilliseconds);
        }

        static void Save(Script script, string name)
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(name, FileMode.Create)))
                {
                    bw.Write(script.Compile());
                    bw.Close();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot create file.");
                return;
            }
        }

        static Script Load(string name)
        {
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(name, FileMode.Open)))
                {
                    Script script = new Script(br.ReadAllBytes());
                    return script;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot open file.");
                return null;
            }
        }
    }

    static class ExtensionMethod
    {
        public static T[] ToArray<T>(this List<T> values)
        {
            int itemcount = values.Count;
            T[] result = new T[itemcount];
            for (int i = 0; i < itemcount; i++)
            {
                result[i] = values[i];
            }
            return result;
        }

        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
    }
    
}
