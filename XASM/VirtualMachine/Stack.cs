using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XASM.VirtualMachine
{
    public class Stack
    {
        private Value[] stack;
        public int topStackIndex { get; set; } = 0;

        public Value this[int index]
        {
            get { return stack[index]; }
            set { stack[index].Assign(value); }
        }

        public Stack(int globalDataSize)
        {
            stack = new Value[1000];
            for (int i = 0; i < 1000; i++)
            {
                stack[i] = new Value();
            }
            topStackIndex = globalDataSize;
        }

        public Value Pop()
        {
            return stack[--topStackIndex];
        }

        public int Push(Value value)
        {
            stack[topStackIndex].Assign(value);
            return ++topStackIndex;
        }

        public Value GetReturnValue()
        {
            return stack[0];
        }
    }
}
