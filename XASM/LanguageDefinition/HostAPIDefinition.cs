using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XASM
{
    public abstract class HostAPILibrary
    {
        public string HAPILibraryName { get; protected set; }
        protected TextReader inputStream;
        protected TextWriter outputStream;

        public HostAPILibrary(TextReader input = null, TextWriter output = null,string hapilibname = null)
        {
            inputStream = input;
            outputStream = output;
            HAPILibraryName = hapilibname;
        }

        public bool ContainsHostAPI(string name)
        {
            return GetAllHostAPI().FirstOrDefault(hapi =>
            {
                return string.Compare(hapi.HAPIname, name, true) == 0;
            }) != null;
        }

        public HostAPI[] GetAllHostAPI()
        {
            List<HostAPI> result = new List<HostAPI>();
            foreach (var methodinfo in GetHapiMethod())
            {
                var temp = methodinfo.GetCustomAttributes().FirstOrDefault(attr => { return attr.GetType() == typeof(HostAPI); }) as HostAPI;
                if (temp != null)
                {
                    result.Add(temp);
                }
            }
            return result.ToArray<HostAPI>();
        }

        public IEnumerable<MethodInfo> GetHapiMethod()
        {
            return GetType().GetMethods().Where(methodInfo => methodInfo.GetCustomAttributes().Any(attr => { return attr.GetType() == typeof(HostAPI); }));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HostAPI : Attribute
    {
        public string HAPIname { get; private set; }
        public int paramCount { get; private set; }
        public bool isReturn { get; private set; }
        public string description { get; private set; }

        public HostAPI(string hapiname = null, int paramcount = 1,bool isreturn = false,string desc = null)
        {
            HAPIname = hapiname;
            paramCount = paramcount;
            isReturn = isreturn;
            description = desc;
        }

        public Function GetFunctionInfo()
        {
            return new Function(0, paramCount, 0, HAPIname);
        }
    }
}
