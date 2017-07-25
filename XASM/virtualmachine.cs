using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XASM.VirtualMachine
{
    public class virtualmachine
    {
        Value[] stack;
        int topStackIndex = 0;
        Script script;
        Dictionary<string, Action> HAPI_table;
        Stack<Function> functionStack;
        Function currFunc
        {
            get
            {
                return functionStack.Peek();
            }
            set
            {
                functionStack.Push(value);
            }
        }

        StringBuilder stacklog = new StringBuilder();

        public void InitTestScript()
        {
            string[] code;

            #region test mov add push callhost
            /* mov reg 10
             * add reg 5
             * push reg
             * callhost "Print"
             */
            //instrs = new Instruction[4];
            ////copy a value type interger of 10 into the registry
            ////the registry is now have a value type interger of 10
            //instrs[0] = new Instruction(OpCode.mov, registry, new Value(10));
            ////add a valu type interger of 5 to the registry
            ////the registry is now have a value type interger of 15
            //instrs[1] = new Instruction(OpCode.add, registry, new Value(5));
            ////push the value that the registry have to the stack
            ////now stack[0] have a value type interger of 15
            //instrs[2] = new Instruction(OpCode.push, registry);
            ////call a HAPI function called "Print"
            //instrs[3] = new Instruction(OpCode.callhost, new Value("Print"));
            #endregion

            #region test jump



            /*0 mov stack<0> 1
             *1 jumpEqual stack<0> 10 3
             *2 jump 0
             *3 callhost "Print"
             */
            //instrs = new Instruction[3];
            //instrs[0] = new Instruction(OpCode.mov, new Value(0,ValType.stackReference), new Value(1));
            //instrs[1] = new Instruction(OpCode.je, new Value(0, ValType.stackReference), new Value(10), new Value(3));
            //instrs[2] = new Instruction(OpCode.jmp, new Value(0));
            //instrs[3] = new Instruction(OpCode.callhost, new Value("Print"));
            #endregion

            #region test stack
            /*0 push 0
             *1 push 1
             *2 push 2
             *3 push 3
             *4 mov stack[0] stack[2]
             *5 mov stack[1] stack[3]
             *6 callhost "Print"
             *7 callhost "Print"
             *8 callhost "Print"
             *9 callhost "Print"
             */
            //instrs = new Instruction[10];
            //instrs[0] = new Instruction(OpCode.push, new Value(0));
            //instrs[1] = new Instruction(OpCode.push, new Value(1));
            //instrs[2] = new Instruction(OpCode.push, new Value(2));
            //instrs[3] = new Instruction(OpCode.push, new Value(3));
            //instrs[4] = new Instruction(OpCode.mov, new Value(0, ValType.stackReference), new Value(2,ValType.stackReference));
            //instrs[5] = new Instruction(OpCode.mov, new Value(1, ValType.stackReference), new Value(3,ValType.stackReference));
            //instrs[6] = new Instruction(OpCode.callhost, new Value("Print"));
            //instrs[7] = new Instruction(OpCode.callhost, new Value("Print"));
            //instrs[8] = new Instruction(OpCode.callhost, new Value("Print"));
            //instrs[9] = new Instruction(OpCode.callhost, new Value("Print"));
            #endregion

            #region test string operation
            /*0 mov reg "1234567890"
             *1 push reg
             *2 callhost "Print"
             *3 push reg
             *4 push 'a'
             *5 setchar 0 stack[0] stack[1]
             *6 getchar 1 stack[1] stack[0]
             *7 setchar 2 stack[0] stack[1]
             *8 callhost "Print"
             *9 callhost "Print"
             */

            //instrs = new Instruction[10];
            //instrs[0] = new Instruction(OpCode.push, new Value(0,ValType.stringLiteral));
            //instrs[1] = new Instruction(OpCode.callhost, new Value("Print"));
            //instrs[2] = new Instruction(OpCode.push, new Value(0, ValType.stringLiteral));
            //instrs[3] = new Instruction(OpCode.push, new Value('a'));
            //instrs[4] = new Instruction(OpCode.setchar, new Value(0), new Value(0, ValType.stackReference), new Value(1, ValType.stackReference));
            //instrs[5] = new Instruction(OpCode.getchar, new Value(1, ValType.stackReference), new Value(0, ValType.stackReference), new Value(1));
            //instrs[6] = new Instruction(OpCode.setchar, new Value(2), new Value(0, ValType.stackReference), new Value(1, ValType.stackReference));
            //instrs[7] = new Instruction(OpCode.callhost, new Value(0));
            //instrs[8] = new Instruction(OpCode.callhost, new Value(0));
            //instrs[9] = new Instruction(OpCode.ret);

            //functable = new Function[0];

            //stringtable = new string[1];
            //stringtable[0] = "1234567890";

            //hapitable = new string[1];
            //hapitable[0] = "Print";
            #endregion;

            #region test global data size
            code = new string[] {
                "var a",
                "func main",
                "mov a \"lala\"",
                "callhost Print",
                "ret",
                "exit 0"
            };

            script = new Script();
            script.globalDataSize = 1;
            script.mainFuncIndex = 0;

            script.instrs = new Instruction[] {
                new Instruction(OpCode.mov, new Value(0, ValType.stackReference), new Value(0,ValType.stringLiteral)),
                new Instruction(OpCode.callhost, new Value(0)),
                new Instruction(OpCode.ret),
                new Instruction(OpCode.exit,new Value(0))
            };

            script.functiontable = new Function[]
            {
                new Function(0,0,0,"main")
            };

            script.stringtable = new string[]
            {
                "lala"
            };
            script.hapitable = new string[]
            {
                "Print"
            };

            #endregion

            var bytecode = script.Compile();
            script.Load(bytecode);
        }

        public void Load(Script script)
        {
            this.script = script;
            functionStack = new Stack<Function>();
            //InitTestScript();
        }

        public void InitHAPITable()
        {
            HAPI_table = new Dictionary<string, Action>();
            HAPI_table.Add("Print", HAPI_Print);
        }

        public void HAPI_Print()
        {
            Value temp = ResolveValue(Pop());
            Console.WriteLine(temp);
        }

        public void InitStack()
        {
            stack = new Value[1000];
            for (int i = 0; i < 1000; i++)
            {
                stack[i] = new Value();
            }
            topStackIndex = script.globalDataSize;
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

        public int PushStackFrame()
        {
            topStackIndex += currFunc.varCount + 1;
            return topStackIndex;
        }

        public int PopStackFrame()
        {
            topStackIndex -= currFunc.varCount + 1;
            return topStackIndex;
        }

        public Value GetReturnValue()
        {
            return stack[0];
        }

        public Value ResolveValue(Value value)
        {
            switch (value.type)
            {
                case ValType.intergerLiteral:
                case ValType.floatLiteral:
                case ValType.charLiteral:
                case ValType.stringLiteral:
                    return value;
                case ValType.stackReference:
                    return ResolveStackReference(value);
                default:
                    throw new Exception("null");
            }
        }

        public Value ResolveStackReference(Value value)
        {
            if (value.type == ValType.stackReference)
            {
                //absolute stack index
                if (value.i>=0)
                {
                    if (value.i>topStackIndex)
                    {
                        Console.WriteLine("=====");
                        Console.WriteLine(stacklog.ToString());
                        Console.WriteLine("=====");
                        throw new StackOverflowException(string.Format("Trying to access value out of the stack {0} stackReference: {1} {0} topStackIndex: {2}", Environment.NewLine, value.i, topStackIndex));
                    }
                    return stack[value.i];
                }
                //relative stack index
                else
                {
                    if (topStackIndex + value.i<0)
                    {
                        Console.WriteLine("=====");
                        Console.WriteLine(stacklog.ToString());
                        Console.WriteLine("=====");
                        throw new StackOverflowException(string.Format("Trying to access value out of the stack {0} stackReference: {1} {0} topStackIndex: {2}", Environment.NewLine, value.i, topStackIndex));
                    }
                    return stack[topStackIndex + value.i];
                }
            }
            else
            {
                throw new Exception("this is not a stack reference");
            }
        }

        public string PrintStack()
        {
            StringBuilder stringbuilder = new StringBuilder();

            for (int i = topStackIndex - 1; i >= 0; i--)
            {
                stringbuilder.AppendFormat("{0} {1}", i, stack[i].ToString());
                stringbuilder.AppendLine();
            }

            return stringbuilder.ToString();
        }

        public void Run(int functionIndex = -1)
        {
            if (script == null)
            {
                Console.WriteLine("No script has been loaded");
                return;
            }

            InitStack();
            InitHAPITable();

            int n = script.Length;
            int instrCounter;

            if (script.mainFuncIndex == -1)
            {
                if (functionIndex == -1)
                {
                    currFunc = script.functiontable[0];
                }
                else
                {
                    currFunc = script.functiontable[functionIndex];
                }
            }
            else
            {
                currFunc = script.functiontable[script.mainFuncIndex];
            }
            instrCounter = currFunc.entryPoint;
            PushStackFrame();

            long pauseDuration = 0;
            bool isExit = false;
            int exitCode = 0;
            while (!isExit)
            {

                if (pauseDuration != 0)
                {
                    if (GetCurrentTime() > pauseDuration)
                    {
                        pauseDuration = 0;
                    }
                    else
                    {
                        continue;
                    }
                }

                bool isJump = false;
                Value firstOp;
                Value secondOp;
                Value thirdOp;
                Value temp;
                Instruction instr = script[instrCounter];
                switch (instr.opcode)
                {
                    #region 3 operands instruction
                    case OpCode.je:
                    case OpCode.jne:
                    case OpCode.jg:
                    case OpCode.jl:
                    case OpCode.jge:
                    case OpCode.jle:

                        firstOp = ResolveValue(instr.GetOperand(0));
                        secondOp = ResolveValue(instr.GetOperand(1));
                        thirdOp = ResolveValue(instr.GetOperand(2));
                        switch (instr.opcode)
                        {
                            case OpCode.je:
                                if (firstOp.i == secondOp.i)
                                {
                                    instrCounter = thirdOp.i;
                                    isJump = true;
                                }
                                break;
                            case OpCode.jne:
                                if (firstOp.i != secondOp.i)
                                {
                                    instrCounter = thirdOp.i;
                                    isJump = true;
                                }
                                break;
                            case OpCode.jg:
                                if (firstOp.i > secondOp.i)
                                {
                                    instrCounter = thirdOp.i;
                                    isJump = true;
                                }
                                break;
                            case OpCode.jl:
                                if (firstOp.i < secondOp.i)
                                {
                                    instrCounter = thirdOp.i;
                                    isJump = true;
                                }
                                break;
                            case OpCode.jge:
                                if (firstOp.i >= secondOp.i)
                                {
                                    instrCounter = thirdOp.i;
                                    isJump = true;
                                }
                                break;
                            case OpCode.jle:
                                if (firstOp.i <= secondOp.i)
                                {
                                    instrCounter = thirdOp.i;
                                    isJump = true;
                                }
                                break;
                        }
                        break;


                    case OpCode.getchar:
                    case OpCode.setchar:

                        firstOp = ResolveValue(instr.GetOperand(0));
                        secondOp = ResolveValue(instr.GetOperand(1));
                        thirdOp = ResolveValue(instr.GetOperand(2));

                        switch (instr.opcode)
                        {
                            case OpCode.getchar:
                                temp = new Value(secondOp.s[thirdOp.i]);
                                firstOp.Assign(temp);
                                break;
                            case OpCode.setchar:
                                var carr = secondOp.s.ToCharArray();
                                carr[firstOp.i] = thirdOp.c;
                                secondOp.s = new string(carr);
                                break;
                        }
                        break;
                    #endregion

                    #region 2 operands instruction
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

                    case OpCode.gettype:

                        firstOp = ResolveValue(instr.GetOperand(0));
                        secondOp = ResolveValue(instr.GetOperand(1));

                        switch (instr.opcode)
                        {
                            case OpCode.mov:
                                firstOp.Assign(secondOp);
                                break;
                            //todo calculate float
                            case OpCode.add:
                                temp = new Value(firstOp.i);
                                temp.i += secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.sub:
                                temp = new Value(firstOp.i);
                                temp.i -= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.mul:
                                temp = new Value(firstOp.i);
                                temp.i *= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.div:
                                temp = new Value(firstOp.i);
                                temp.i /= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.mod:
                                temp = new Value(firstOp.i);
                                temp.i %= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.exp:
                                temp = new Value(firstOp.i);
                                temp.i += secondOp.i;
                                firstOp.Copy(temp);
                                break;

                            case OpCode.and:
                                temp = new Value(firstOp.i);
                                temp.i &= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.or:
                                temp = new Value(firstOp.i);
                                temp.i |= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.xor:
                                temp = new Value(firstOp.i);
                                temp.i ^= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.shl:
                                temp = new Value(firstOp.i);
                                temp.i <<= secondOp.i;
                                firstOp.Copy(temp);
                                break;
                            case OpCode.shr:
                                temp = new Value(firstOp.i);
                                temp.i >>= secondOp.i;
                                firstOp.Copy(temp);
                                break;

                            case OpCode.concat:
                                temp = new Value(firstOp.s);
                                temp.s += secondOp.s;
                                firstOp.Copy(temp);
                                break;

                            case OpCode.gettype:
                                temp = new Value(secondOp.type.ToString());
                                firstOp.Copy(temp);
                                break;
                        }
                        break;
                    #endregion

                    #region 1 operand instruction
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

                        firstOp = ResolveValue(instr.GetOperand(0));
                        switch (instr.opcode)
                        {
                            case OpCode.call:
                                isJump = true;
                                //call function

                                //get the function that is called
                                currFunc = script.functiontable[firstOp.i];

                                //push its stack frame
                                PushStackFrame();

                                //save the caller to jump back later
                                stack[topStackIndex - currFunc.varCount - 1] = new Value(instrCounter + 1);

                                //jump to the function entrypoint
                                instrCounter = currFunc.entryPoint;
                                break;

                            case OpCode.callhost:
                                string hapi_name = firstOp.s;
                                HAPI_table[hapi_name].Invoke();
                                break;

                            case OpCode.push:
                                Push(firstOp);
                                break;
                            case OpCode.pop:
                                temp = Pop();
                                firstOp.Assign(temp);
                                break;

                            case OpCode.jmp:
                                instrCounter = firstOp.i;
                                isJump = true;
                                break;

                            case OpCode.neg:
                                firstOp.i = -firstOp.i;
                                break;
                            case OpCode.inc:
                                firstOp.i++;
                                break;
                            case OpCode.dec:
                                firstOp.i--;
                                break;

                            case OpCode.not:
                                firstOp.i = ~firstOp.i;
                                break;

                            case OpCode.pause:
                                pauseDuration = GetCurrentTime() + firstOp.i;
                                break;
                            case OpCode.exit:
                                isExit = true;
                                exitCode = firstOp.i;
                                break;
                        }
                        break;
                    #endregion

                    case OpCode.ret:
                        try
                        {
                            //jump back to caller
                            instrCounter = stack[topStackIndex - currFunc.varCount - 1].i;
                            isJump = true;
                        }
                        catch
                        {
                            Console.WriteLine(stacklog.ToString());
                        }

                        //pop currfunc stack frame
                        PopStackFrame();

                        //pop the currFunc of the functionStack
                        functionStack.Pop();

                        if (functionStack.Count == 0)
                        {
                            isExit = true;
                        }

                        break;

                    default:
                        break;
                }

                if (!isJump)
                {
                    instrCounter++;
                }

                stacklog.AppendLine(instr.ToString());
                stacklog.AppendLine(PrintStack());
            }

            //exit code
            Console.WriteLine("exit code : " + exitCode);

            File.WriteAllText("stacklog.txt", stacklog.ToString());
        }

        private long GetCurrentTime()
        {
            return DateTime.Now.Millisecond;
        }
    }
}
