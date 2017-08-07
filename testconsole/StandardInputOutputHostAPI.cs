using System.IO;
using XASM;
using XASM.VirtualMachine;

namespace testconsole
{
    public class StandardInputOutputHostAPI : HostAPILibrary
    {
        public StandardInputOutputHostAPI(TextReader input,TextWriter output) : base(input, output)
        {
            HAPILibraryName = GetType().Name;
        }

        [HostAPI("Print")]
        public void Print(Stack stack)
        {
            var temp = stack.Pop();
            outputStream.WriteLine(temp);
        }

        [HostAPI("Read",0,true)]
        public void Read(Stack stack)
        {
            outputStream.WriteLine("Xin nhap");
            var temp = inputStream.ReadLine();
            stack.GetReturnValue().Copy(new Value(temp));
        }
    }
}
