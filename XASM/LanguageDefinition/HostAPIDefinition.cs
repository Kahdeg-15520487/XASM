using System;
using System.IO;

namespace XASM
{
    public abstract class HostAPILibrary
    {
        protected TextReader inputStream;
        protected TextWriter outputStream;

        public HostAPILibrary(TextReader input, TextWriter output)
        {
            inputStream = input;
            outputStream = output;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HostAPI : Attribute
    {
        private string HAPIname;
        private int paramCount;
        private bool isReturn;

        public HostAPI(string hapiname = null, int paramcount = 1,bool isreturn = false)
        {
            HAPIname = hapiname;
            paramCount = paramcount;
            isReturn = isreturn;
        }
    }
}
