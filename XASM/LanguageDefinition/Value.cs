namespace XASM
{
    public enum ValType
    {
        intergerLiteral,
        floatLiteral,
        charLiteral,
        stringLiteral,
        stackReference,
        arrayIndex,
        stackIndex
    }

    /// <summary>
    /// A value is everything from literal value to variable.
    /// </summary>
    public class Value
    {
        public ValType type;
        public int i;
        public float f;
        public char c;
        public string s;
        public int arrid;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        public Value()
        {
            type = ValType.intergerLiteral;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="t">The type.</param>
        public Value(ValType t)
        {
            type = t;
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="i">The intergerLiteral.</param>
        /// <param name="t">The type.</param>
        public Value(int i, ValType t = ValType.intergerLiteral)
        {
            type = t;
            this.i = i;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="f">The floatLiteral.</param>
        /// <param name="t">The type.</param>
        public Value (float f, ValType t = ValType.floatLiteral)
        {
            type = t;
            this.f = f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="c">The charLiteral.</param>
        /// <param name="t">The type.</param>
        public Value (char c,ValType t = ValType.charLiteral)
        {
            type = t;
            this.c = c;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="s">The stringLiteral.</param>
        /// <param name="t">The type.</param>
        public Value(string s, ValType t = ValType.stringLiteral)
        {
            type = t;
            this.s = s;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="i">The index of the first item in the array.</param>
        /// <param name="arrayIndex">Index of the item in the array.</param>
        /// <param name="t">The type.</param>
        public Value(int i,int arrayIndex, ValType t = ValType.arrayIndex)
        {
            type = t;
            this.i = i;
            this.arrid = arrayIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class from another Value object.
        /// </summary>
        /// <param name="other">The other Value object.</param>
        public Value(Value other)
        {
            type = other.type;
            i = other.i;
            f = other.f;
            c = other.c;
            s = other.s;
            arrid = other.arrid;
        }
        #endregion

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
        /// Will override type and value of this value
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
        /// Will attempt to convert string into target value
        /// </summary>
        /// <param name="other">The other value.</param>
        public void Copy(Value other)
        {
            switch (type)
            {
                case ValType.intergerLiteral:
                    switch (other.type)
                    {
                        case ValType.intergerLiteral:
                            i = other.i;
                            break;
                        case ValType.floatLiteral:
                            i = (int)other.f;
                            break;
                        case ValType.stringLiteral:
                            int.TryParse(other.s, out i);
                            break;
                    }
                    break;
                case ValType.floatLiteral:
                    switch (other.type)
                    {
                        case ValType.intergerLiteral:
                            f = other.i;
                            break;
                        case ValType.floatLiteral:
                            f = other.f;
                            break;
                        case ValType.stringLiteral:
                            float.TryParse(other.s, out f);
                            break;
                    }
                    break;
                case ValType.charLiteral:
                    switch (other.type)
                    {
                        case ValType.intergerLiteral:
                            c = (char)other.i;
                            break;
                        case ValType.charLiteral:
                            c = other.c;
                            break;
                    }
                    break;
                case ValType.stringLiteral:
                    switch (other.type)
                    {
                        case ValType.intergerLiteral:
                            s = other.i.ToString();
                            break;
                        case ValType.floatLiteral:
                            s = other.f.ToString();
                            break;
                        case ValType.charLiteral:
                            s = other.c.ToString();
                            break;
                        case ValType.stringLiteral:
                            s = other.s;
                            break;
                    }
                    break;
                case ValType.stackReference:
                    i = other.i;
                    break;
                case ValType.arrayIndex:
                    i = other.i;
                    arrid = other.arrid;
                    break;
                case ValType.stackIndex:
                    i = other.i;
                    break;
            }
        }


        #region Check type
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

        #endregion

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
                case ValType.stackIndex:
                    return "<" + i + ">";
                case ValType.arrayIndex:
                    return "<" + i + ", " + arrid + ">";
                default:
                    return "null";
            }
        }
    }
}