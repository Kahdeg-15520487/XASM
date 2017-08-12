using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XASM;
using XASM.VirtualMachine;

namespace testconsole
{
    class NumberHostAPI : HostAPILibrary
    {
        Random randomizer;
        public NumberHostAPI(TextReader input = null, TextWriter output = null) : base(input, output)
        {
            HAPILibraryName = "number";
            randomizer = new Random();
        }

        [HostAPI("GetRandom",0,true)]
        public void GetRandom(Stack stack)
        {
            var result = new Value(randomizer.Next());
            stack.GetReturnValue().Assign(result);
        }

        [HostAPI("GetRandomUpTo",1,true)]
        public void GetRandomUpTo(Stack stack)
        {
            var param0 = stack.Pop();
            var result = new Value(randomizer.Next(param0.i));
            stack.GetReturnValue().Assign(result);
        }

        [HostAPI("GetRandomBetween",2,true)]
        public void GetRandomBetween(Stack stack)
        {
            var param1 = stack.Pop();
            var param0 = stack.Pop();
            var result = new Value(randomizer.Next(param0.i, param1.i));
            stack.GetReturnValue().Assign(result);
        }
    }
}
