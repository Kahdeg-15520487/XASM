using System.Text;

namespace XASM.VirtualMachine
{
    public enum Error
    {
        StackOverFlowUpperBound,
        StackOverFlowLowerBound,
        TypeMismatch
    }

    public class RuntimeError
    {
        public Error error { get; private set; }
        public string stacklog { get; private set; }
        public string message { get; private set; }

        public RuntimeError(string stacklog,string message = null)
        {
            this.stacklog = stacklog;
            this.message = message;
        }

        public RuntimeError(Error error,string stacklog,string message = null)
        {
            this.error = error;
            this.stacklog = stacklog;
            this.message = message;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine(error.ToString());
            result.AppendLine(message);
            result.AppendLine("=====");
            result.AppendLine(stacklog);
            result.AppendLine("=====");

            return result.ToString();
        }
    }
}
