using System;
using System.IO;

namespace XASM
{
    public class Script
    {
        public Instruction[] instrs;
        public Function[] functiontable;
        public string[] stringtable;
        public string[] hapitable;

        public int globalDataSize;
        public int mainFuncIndex;

        public Instruction this[int index]
        {
            get { return instrs[index]; }
            set { instrs[index] = value; }
        }

        public int Length { get { return instrs.GetLength(0); } }

        public Script() { }
        public Script(Instruction[] instrs,Function[] functiontable, string[] stringtable,string[] hapitable,int globalDataSize = 0, int mainFuncIndex = 0)
        {
            this.instrs = instrs;
            this.functiontable = functiontable;
            this.stringtable = stringtable;
            this.hapitable = hapitable;
        }
        public Script(byte[] bytecode)
        {
            this.Load(bytecode);
        }

        public void RegisterString(params string[] strs)
        {
            stringtable = new string[strs.GetLength(0)];
            for (int i = 0; i < strs.GetLength(0); i++)
            {
                stringtable[i] = strs[i];
            }
        }

        public void RegisterHAPI(params string[] hapis)
        {
            hapitable = new string[hapis.GetLength(0)];
            for (int i = 0; i < hapis.GetLength(0); i++)
            {
                hapitable[i] = hapis[i];
            }
        }

        public bool Load (byte[] code)
        {
            using (MemoryStream ms = new MemoryStream(code))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    OpCode opcode;
                    Value[] operands;
                    ValType optype;
                    int instrcount;
                    int funccount;
                    int stringcount;
                    int hapicount;
                    try
                    {
                        //read binary header
                        globalDataSize = br.ReadInt32();
                        mainFuncIndex = br.ReadInt32();
                        instrcount = br.ReadInt32();
                        funccount = br.ReadInt32();
                        stringcount = br.ReadInt32();
                        hapicount = br.ReadInt32();

                        //read instructions
                        instrs = new Instruction[instrcount];
                        for (int ic = 0; ic < instrcount; ic++)
                        {
                            opcode = (OpCode)br.ReadByte();
                            operands = new Value[opcode.GetOpCount()];
                            for (int i = 0; i < operands.GetLength(0); i++)
                            {
                                optype = (ValType)br.ReadByte();
                                switch (optype)
                                {
                                    case ValType.intergerLiteral:
                                        operands[i] = new Value(br.ReadInt32());
                                        break;
                                    case ValType.floatLiteral:
                                        operands[i] = new Value(br.ReadSingle());
                                        break;
                                    case ValType.charLiteral:
                                        operands[i] = new Value(br.ReadChar());
                                        break;
                                    case ValType.stringLiteral:
                                        operands[i] = new Value(br.ReadInt32(), ValType.stringLiteral);
                                        break;
                                    case ValType.stackReference:
                                        operands[i] = new Value(br.ReadInt32(), ValType.stackReference);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            instrs[ic] = new Instruction(opcode, operands);
                        }

                        //read functiont table
                        functiontable = new Function[funccount];
                        for (int i = 0; i < funccount; i++)
                        {
                            functiontable[i] = new Function(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(),br.ReadString());
                        }

                        //read string table
                        stringtable = new string[stringcount];
                        for (int i = 0; i < stringcount; i++)
                        {
                            stringtable[i] = br.ReadString();
                        }

                        //read hapi table
                        hapitable = new string[hapicount];
                        for (int i = 0; i < hapicount; i++)
                        {
                            hapitable[i] = br.ReadString();
                        }

                        //scan and replace stringLiteral and hapi call
                        for (int ic = 0; ic < instrs.GetLength(0); ic++)
                        {
                            Instruction instr = instrs[ic];
                            if (instr.opcode == OpCode.callhost)
                            {
                                instr.operands[0].Assign(new Value(hapitable[instr.operands[0].i]));
                                continue;
                            }

                            for (int i=0;i<instr.operands.GetLength(0);i++)
                            {
                                var val = instr.operands[i];
                                if (val.type == ValType.stringLiteral)
                                {
                                    val.s = stringtable[val.i];
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Can not load specified binary");
                        Console.WriteLine(e.Message);
                        return false;
                    }
                    return true;
                }
            }
        }

        public static byte[] Compile(Instruction[] instrs, Function[] functiontable, string[] stringtable, string[] hapitable, int globaldatasize = 0,int mainfuncindex = -1)
        {
            byte[] result = new byte[1000];

            /* binary layout
             * xxxx         <- global data size         4 byte
             * xxxx         <- main function index      4 byte
             * xxxx         <- instruction count        4 byte
             * xxxx         <- function count           4 byte
             * xxxx         <- string table count       4 byte
             * xxxx         <- hapi table count         4 byte
             * {
             *  x           <- opcode                   1 byte
             *   [{
             *     x        <- operand type             1 byte
             *     (
             *      xxxx    <- interger literal         4 byte
             *     |xxxx    <- float lieral             4 byte
             *     |xx      <- char literal             2 byte
             *     |xxxx    <- string table index       4 byte
             *     )
             *   }]
             * }
             * [{
             * xxxx         <- entry point              4 byte
             * xxxx         <- param count              4 byte
             * xxxx         <- local var count          4 byte
             * x..x         <- function name
             * }]
             * [{
             * x..x         <- string
             * }]
             * [{
             * x..x         <- hapi name(string)
             * }]
             */

            using (MemoryStream ms = new MemoryStream(result))
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    //write binary header
                    bw.Write(globaldatasize);
                    bw.Write(mainfuncindex);
                    bw.Write(instrs.GetLength(0));
                    bw.Write(functiontable.GetLength(0));
                    bw.Write(stringtable.GetLength(0));
                    bw.Write(hapitable.GetLength(0));

                    //write instructions
                    foreach (Instruction instr in instrs)
                    {
                        bw.Write((byte)instr.opcode);
                        for (int i = 0; i < instr.operands.GetLength(0); i++)
                        {
                            Value temp = instr.operands[i];
                            bw.Write((byte)temp.type);
                            switch (temp.type)
                            {
                                case ValType.intergerLiteral:
                                    bw.Write(temp.i);
                                    break;
                                case ValType.floatLiteral:
                                    bw.Write(temp.f);
                                    break;
                                case ValType.charLiteral:
                                    bw.Write(temp.c);
                                    break;
                                case ValType.stringLiteral:
                                    bw.Write(temp.i);
                                    break;
                                case ValType.stackReference:
                                    bw.Write(temp.i);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    //write functiontable
                    foreach (Function func in functiontable)
                    {
                        bw.Write(func.entryPoint);
                        bw.Write(func.paramCount);
                        bw.Write(func.varCount);
                        bw.Write(func.funcName);
                    }

                    //write stringtable
                    foreach (string str in stringtable)
                    {
                        bw.Write(str);
                    }

                    //write hapitable
                    foreach (string hapi in hapitable)
                    {
                        bw.Write(hapi);
                    }
                }
            }

            return result;
        }

        public byte[] Compile()
        {
            return Compile(instrs, functiontable, stringtable, hapitable, globalDataSize, mainFuncIndex);
        }
    }
}
