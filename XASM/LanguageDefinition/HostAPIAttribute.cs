using System;
namespace XASM
{
    [AttributeUsage(AttributeTargets.Method)]
    class HostAPI : Attribute
    {
        private string HAPIname;
        private int popCount;
        private bool isReturn;

        public HostAPI(string hapiname = null, int popcount = 1,bool isreturn = false)
        {
            HAPIname = hapiname;
            popCount = popcount;
            isReturn = isreturn;
        }
    }
}
