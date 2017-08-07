using System;
using System.IO;

namespace XASM
{
    public abstract class HostAPILibrary
    {
        public string HAPILibraryName { get; protected set; }
        protected TextReader inputStream;
        protected TextWriter outputStream;

        public HostAPILibrary(TextReader input, TextWriter output,string hapilibname = null)
        {
            inputStream = input;
            outputStream = output;
            HAPILibraryName = hapilibname;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HostAPI : Attribute
    {
        public string HAPIname { get; private set; }
        public int paramCount { get; private set; }
        public bool isReturn { get; private set; }

        public HostAPI(string hapiname = null, int paramcount = 1,bool isreturn = false)
        {
            HAPIname = hapiname;
            paramCount = paramcount;
            isReturn = isreturn;
        }

        public Function GetFunctionInfo()
        {
            return new Function(0, paramCount, 0, HAPIname);
        }
    }
}
