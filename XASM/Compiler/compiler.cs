using System.IO;

namespace XASM.Compiler
{
    public class compiler
    {
        public TextWriter outputStream = System.Console.Out;
        public Script Compile(Stream codeStream,bool verbose = false)
        {
            Errors errors = new Errors(outputStream);
            ScriptEmitter emitter = new ScriptEmitter();
            Scanner scanner = new Scanner(codeStream);
            Parser parser = new Parser(scanner, emitter, errors, verbose);

            parser.Parse();
            outputStream.WriteLine("Errors count: " + parser.errors.count);

            if (errors.count ==0)
            {
                outputStream.WriteLine("Compilation completed!");
                outputStream.Close();
                return emitter.Emit();
            }
            else
            {
                outputStream.Close();
                return null;
            }
        }

        public Script Compile(string codeFilePath, bool verbose = false)
        {
            Errors errors = new Errors(outputStream);
            ScriptEmitter emitter = new ScriptEmitter();
            Scanner scanner = new Scanner(codeFilePath);
            Parser parser = new Parser(scanner, emitter, errors, verbose);

            parser.Parse();
            outputStream.WriteLine("Errors count: " + parser.errors.count);

            if (errors.count == 0)
            {
                outputStream.WriteLine("Compilation completed!");
                outputStream.Close();
                return emitter.Emit();
            }
            else
            {
                outputStream.Close();
                return null;
            }
        }
    }
}
