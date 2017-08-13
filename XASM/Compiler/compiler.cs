using System.IO;

namespace XASM.Compiler
{
    /// <summary>
    /// The compiler which will compile XASM sourcecode to an object of type Script
    /// </summary>
    public class compiler
    {
        /// <summary>
        /// The output stream
        /// </summary>
        public TextWriter outputStream = System.Console.Out;

        /// <summary>
        /// Compiles the specified code stream.
        /// </summary>
        /// <param name="codeStream">The code stream.</param>
        /// <param name="verbose">if set to <c>true</c> [verbose].</param>
        /// <param name="hapilibs">The hapilibs.</param>
        /// <returns></returns>
        public Script Compile(Stream codeStream, bool verbose = false, params HostAPILibrary[] hapilibs)
        {
            Errors errors = new Errors(outputStream);
            ScriptEmitter emitter = new ScriptEmitter();
            Scanner scanner = new Scanner(codeStream);
            Parser parser = new Parser(scanner, emitter, errors, verbose, hapilibs);

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

        /// <summary>
        /// Compiles the specified code file path.
        /// </summary>
        /// <param name="codeFilePath">The code file path.</param>
        /// <param name="verbose">if set to <c>true</c> [verbose].</param>
        /// <param name="hapilibs">The hapilibs.</param>
        /// <returns></returns>
        public Script Compile(string codeFilePath, bool verbose = false, params HostAPILibrary[] hapilibs)
        {
            Errors errors = new Errors(outputStream);
            ScriptEmitter emitter = new ScriptEmitter();
            Scanner scanner = new Scanner(codeFilePath);
            Parser parser = new Parser(scanner, emitter, errors, verbose, hapilibs);

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