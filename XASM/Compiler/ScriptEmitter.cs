using System;
using System.Collections.Generic;

namespace XASM.Compiler
{
    public class ScriptEmitter
    {
        private List<Instruction> instrs;
        private List<Function> functiontable;
        private List<string> stringtable;
        private List<string> hapitable;

        private Dictionary<string, int> lineLabels;

        public int globalDataSize;
        public int mainFuncIndex = -1;

        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <value>
        /// The current line.
        /// </value>
        public int CurrentLine { get { return instrs.Count; } }

        public ScriptEmitter()
        {
            instrs = new List<Instruction>();
            functiontable = new List<Function>();
            stringtable = new List<string>();
            hapitable = new List<string>();
            lineLabels = new Dictionary<string, int>();
        }

        /// <summary>
        /// Emits the specified instr.
        /// </summary>
        /// <param name="instr">The instr.</param>
        /// <returns>The line that has just been added</returns>
        public int AddInstruction(Instruction instr)
        {
            instrs.Add(instr);
            return instrs.Count - 1;
        }

        /// <summary>
        /// Adds the function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public int AddFunction(Function func)
        {
            functiontable.Add(func);
            return functiontable.Count - 1;
        }

        /// <summary>
        /// Gets the index of the function.
        /// </summary>
        /// <param name="funcname">The function's name.</param>
        /// <returns></returns>
        public int GetFunctionIndex(string funcname)
        {
            return functiontable.FindIndex(f => { return f.funcName.CompareTo(funcname) == 0; });
        }

        /// <summary>
        /// Adds the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public int AddString(string str)
        {
            if (!stringtable.Contains(str))
            {
                stringtable.Add(str);
                return stringtable.Count - 1;
            }
            else
            {
                return stringtable.FindIndex(s => { return string.Compare(s, str) == 0; });
            }
        }

        /// <summary>
        /// Adds the host api.
        /// </summary>
        /// <param name="hapi">The host api name.</param>
        /// <returns></returns>
        public int AddHAPI(string hapi)
        {
            if (!hapitable.Contains(hapi))
            {
                hapitable.Add(hapi);
                return hapitable.Count - 1;
            }
            else
            {
                return hapitable.FindIndex(s => { return string.Compare(s, hapi) == 0; });
            }
        }

        /// <summary>
        /// Adds the line label.
        /// </summary>
        /// <param name="linelabel">The linelabel.</param>
        public void AddLineLabel(string linelabel,int line)
        {
            try
            {
                lineLabels.Add(linelabel, line);
            }
            catch (Exception e)
            {
                Console.WriteLine(linelabel);
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public Script Emit()
        {
            Script script = new Script();

            script.functiontable = new Function[functiontable.Count];
            for (int i = 0; i < functiontable.Count; i++)
            {
                script.functiontable[i] = functiontable[i];
            }

            script.stringtable = new string[stringtable.Count];
            for (int i = 0; i < stringtable.Count; i++)
            {
                script.stringtable[i] = stringtable[i];
            }

            script.hapitable = new string[hapitable.Count];
            for (int i = 0; i < hapitable.Count; i++)
            {
                script.hapitable[i] = hapitable[i];
            }

            script.instrs = new Instruction[instrs.Count];
            for (int i = 0; i < instrs.Count; i++)
            {
                script.instrs[i] = instrs[i];
                var instr = script.instrs[i];
                switch (script.instrs[i].opcode)
                {
                    case OpCode.jmp:
                        instr.operands[0] = new Value(lineLabels[instr.operands[0].s]);
                        break;

                    case OpCode.je:
                        instr.operands[2] = new Value(lineLabels[instr.operands[2].s]);
                        break;

                    case OpCode.call:
                        instr.operands[0].i = GetFunctionIndex(instr.operands[0].s);
                        break;

                    default:
                        break;
                }
            }

            script.globalDataSize = globalDataSize;
            script.mainFuncIndex = mainFuncIndex;

            return new Script(script.Compile());
        }
    }
}
