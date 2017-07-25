namespace XASM
{
    static class InstructionExtensionMethod
    {
        public static int GetOpCount(this OpCode oc)
        {
            int ropcount = 0;
            switch (oc)
            {
                // 3 operands
                // op1.int op2.int op3.int
                // compare op1.int and op2.int then jump op3.int th instruction
                case OpCode.je:
                case OpCode.jne:
                case OpCode.jg:
                case OpCode.jl:
                case OpCode.jge:
                case OpCode.jle:

                // op1.char op2.string op3.int
                // get the char at op3.int in op2.string and save result in op1.char
                case OpCode.getchar:
                // op1.int op2.string op3.int
                // set the char at op1.int in op2.string as op3.char
                case OpCode.setchar:

                    ropcount = 3;
                    break;

                //2 operands
                case OpCode.mov:
                case OpCode.add:
                case OpCode.sub:
                case OpCode.mul:
                case OpCode.div:
                case OpCode.mod:
                case OpCode.exp:
                case OpCode.and:
                case OpCode.or:
                case OpCode.xor:
                case OpCode.shl:
                case OpCode.shr:
                case OpCode.concat:

                    ropcount = 2;
                    break;

                //1 operands
                case OpCode.call:
                case OpCode.callhost:
                case OpCode.push:
                case OpCode.pop:
                case OpCode.jmp:
                case OpCode.neg:
                case OpCode.inc:
                case OpCode.dec:
                case OpCode.not:
                case OpCode.pause:
                case OpCode.exit:

                    ropcount = 1;
                    break;

                //0 operands
                case OpCode.ret:

                    ropcount = 0;
                    break;

                default:
                    break;
            }

            return ropcount;
        }
    }
}
