namespace XASM
{
    /// <summary>
    /// A function definition in XASM
    /// </summary>
    public class Function
    {
        public int entryPoint;
        public int paramCount;
        public int varCount;
        public string funcName;
        public Function (int entry,int param,int var,string name)
        {
            entryPoint = entry;
            paramCount = param;
            varCount = var;
            funcName = name;
        }
    }
}
