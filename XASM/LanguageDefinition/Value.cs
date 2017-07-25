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
        private void Init()
        {
            i = 0;
            f = 0;
            c = '\0';
            s = string.Empty;
            arrid = 0;
        }

        /// <summary>
        /// Assigns to the value of other.
        /// </summary>
        /// <param name="other">The other value.</param>
        public void Assign(Value other)
        {
            type = other.type;
            i = other.i;
            f = other.f;
            c = other.c;
            s = other.s;
            arrid = other.arrid;
        }

        /// <summary>
        /// Copies value of other.
        /// </summary>
        /// <param name="other">The other value.</param>
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

        /// <summary>
        /// Determines whether this instance is number.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is number; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNumber()
        {
            return type == ValType.intergerLiteral || type == ValType.floatLiteral;
        }

        /// <summary>
        /// Determines whether this instance is letter.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is letter; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLetter()
        {
            return type == ValType.charLiteral || type == ValType.stringLiteral;
        }

        /// <summary>
        /// Determines whether this instance is interger.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is interger; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInterger()
        {
            return type == ValType.intergerLiteral;
        }

        /// <summary>
        /// Determines whether this instance is float.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is float; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFloat()
        {
            return type == ValType.floatLiteral;
        }

        /// <summary>
        /// Determines whether this instance is character.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is character; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChar()
        {
            return type == ValType.charLiteral;
        }

        /// <summary>
        /// Determines whether this instance is string.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is string; otherwise, <c>false</c>.
        /// </returns>
        public bool IsString()
        {
            return type == ValType.stringLiteral;
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