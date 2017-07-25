using System;
using System.Text;

namespace XASM
{
    public enum OpCode
    {
        /// <summary>
        /// copy value of op2 and put in op1
        /// op1 : stackReference
        /// op2 : any
        /// </summary>
        mov,

        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        add,
        /// <summary>
        /// subtract value of op2 from op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        sub,
        /// <summary>
        /// multiply value of op2 and op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        mul,
        /// <summary>
        /// divide value of op1 by op2 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        div,
        /// <summary>
        /// divide value of op1 and op2 and put the remain in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        mod,
        /// <summary>
        /// calculate the value of op1 to the power of op2 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        exp,
        /// <summary>
        /// get the negative numeric value of op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// op2 : any - charLiteral,stringLiteral
        /// </summary>
        neg,
        /// <summary>
        /// increase value of op1 by 1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// </summary>
        inc,
        /// <summary>
        /// decrease value of op1 by 1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral, floatLiteral
        /// </summary>
        dec,

        /// <summary>
        /// bitwise and value of op2 and op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral
        /// op2 : any - floatLiteral,charLiteral,stringLiteral
        /// </summary>
        and,
        /// <summary>
        /// bitwise or value of op2 and op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral
        /// op2 : any - floatLiteral,charLiteral,stringLiteral
        /// </summary>
        or,
        /// <summary>
        /// bitwise xor value of op2 and op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral
        /// op2 : any - floatLiteral,charLiteral,stringLiteral
        /// </summary>
        xor,
        /// <summary>
        /// bitwise not value of op1 and put the result in op1
        /// op1 : stackReference -> intergerLiteral
        /// </summary>
        not,
        /// <summary>
        /// bitwise shift left value of op1 by op2 and put the result in op1
        /// op1 : stackReference -> intergerLiteral
        /// op2 : any - floatLiteral,charLiteral,stringLiteral
        /// </summary>
        shl,
        /// <summary>
        /// bitwise shift right value of op1 by op2 and put the result in op1
        /// op1 : stackReference -> intergerLiteral
        /// op2 : any - floatLiteral,charLiteral,stringLiteral
        /// </summary>
        shr,

        /// <summary>
        /// concatenate op1 and op2 and put the result in op1
        /// op1 : stackReference -> stringLiteral
        /// op2 : any
        /// </summary>
        concat,
        /// <summary>
        /// get the char in op2 at position op3 and put the result in op1
        /// op1 : stackReference -> charLiteral
        /// op2 : stackReference -> stringLiteral | stringLiteral
        /// op3 : stackReference -> intergerLiteral | intergerLiteral
        /// </summary>
        getchar,
        /// <summary>
        /// set the char in op2 at position op1 as op3 and put the result in op2
        /// op1 : stackReference -> intergerLiteral | intergerLiteral
        /// op2 : stackReference -> stringLiteral | stringLiteral
        /// op3 : stackReference -> charLiteral
        /// </summary>
        setchar,

        /// <summary>
        /// call the function at index op1
        /// op1 : intergerLiteral
        /// </summary>
        call,
        /// <summary>
        /// return from currently executing function
        /// </summary>
        ret,
        /// <summary>
        /// call the HAPI function with the name op1
        /// op1 : stringLiteral
        /// </summary>
        callhost,

        /// <summary>
        /// push op1 on the stack
        /// op1 : any
        /// </summary>
        push,
        /// <summary>
        /// pop the top stack into op1
        /// op1 : stackReference
        /// </summary>
        pop,

        /// <summary>
        /// uncondition jump to line op1
        /// op1 : intergerLiteral
        /// </summary>
        jmp,
        /// <summary>
        /// jump to line op3 if op1 = op2,
        /// op1 : any,
        /// op2 : any,
        /// op3 : intergerLiteral,
        /// op1 and op2 must be of the same type
        /// </summary>
        je,
        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        jne,
        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        jg,
        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        jl,
        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        jge,
        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        jle,

        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        pause,
        /// <summary>
        /// add value of op2 and op1 and put the result in op1
        /// </summary>
        exit,

        /// <summary>
        /// get the op2's type and put the result in op1
        /// op1 : stackReference -> string
        /// op2 : any
        /// </summary>
        gettype
    }

    public class Instruction
    {
        public OpCode opcode;
        public Value[] operands;

        public Instruction(OpCode oc, params Value[] ops)
        {
            if (ops == null)
            {
                throw new NullReferenceException("operands list is null");
            }

            int opcount = ops.GetLength(0);

            opcode = oc;
            if (opcount != opcode.GetOpCount())
            {
                throw new ArgumentOutOfRangeException(opcode + " instruction only accept " + opcode.GetOpCount() + " operand");
            }

            operands = new Value[opcount];
            for (int i = 0; i < opcount; i++)
            {
                operands[i] = ops[i];
            }
        }

        public Value GetOperand(int index)
        {
            return operands[index];
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0} ", opcode.ToString());
            for (int i = 0; i < operands.GetLength(0); i++)
            {
                result.AppendFormat("{0} ", operands[i].ToString());
            }
            return result.ToString();
        }
    }
}