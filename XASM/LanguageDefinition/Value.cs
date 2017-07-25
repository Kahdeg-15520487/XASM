namespace XASM
{
    public enum ValType
    {
        intergerLiteral,
        floatLiteral,
        charLiteral,
        stringLiteral,
        stackReference,
        arrayIndex
    }

    public class Value
    {
        public ValType type;
        public int i;
        public float f;
        public char c;
        public string s;
        public int arrid;
        public Value()
        {
            type = ValType.intergerLiteral;
            Init();
        }
        public Value(ValType t)
        {
            type = t;
            Init();
        }
        public Value(int i, ValType t = ValType.intergerLiteral)
        {
            type = t;
            this.i = i;
        }
        public Value (float f, ValType t = ValType.floatLiteral)
        {
            type = t;
            this.f = f;
        }
        public Value (char c,ValType t = ValType.charLiteral)
        {
            type = t;
            this.c = c;
        }
        public Value(string s, ValType t = ValType.stringLiteral)
        {
            type = t;
            this.s = s;
        }
        public Value(int i,int arrayIndex, ValType t = ValType.arrayIndex)
        {
            type = t;
            this.i = i;
            this.arrid = arrayIndex;
        }
        public Value(Value other)
        {
            type = other.type;
            i = other.i;
            f = other.f;
            c = other.c;
            s = other.s;
            arrid = other.arrid;
        }
        void Init()
        {
            i = 0;
            f = 0;
            c = '\0';
            s = string.Empty;
            arrid = 0;
        }

        //make this Value object the same as other
        public void Assign(Value other)
        {
            type = other.type;
            i = other.i;
            f = other.f;
            c = other.c;
            s = other.s;
            arrid = other.arrid;
        }

        //only copy the content of other
        public void Copy(Value other)
        {
            i = other.i;
            f = other.f;
            c = other.c;
            s = other.s;
            arrid = other.arrid;
        }

        /// <summary>
        /// Determines whether this Value's Type is .
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsType(ValType type)
        {
            return this.type == type;
        }

        public override string ToString()
        {
            switch (type)
            {
                case ValType.intergerLiteral:
                    return i.ToString();
                case ValType.floatLiteral:
                    return f.ToString();
                case ValType.charLiteral:
                    return c.ToString();
                case ValType.stringLiteral:
                    return s;
                case ValType.stackReference:
                    return "stack[" + i + "]";
                case ValType.arrayIndex:
                    return "stack[" + (i + arrid) + "]";
                default:
                    return "null";
            }
        }
    }
}