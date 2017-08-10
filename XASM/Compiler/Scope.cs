using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XASM.Compiler
{
    class Scope
    {
        public Scope fatherScope;
        public Dictionary<string,Scope> childScopes;
        public List<string> variables;
        public List<string> parameters;

        public bool IsGlobalScope { get { return fatherScope == null; } }

        public Scope(Scope father = null)
        {
            fatherScope = father;
            childScopes = new Dictionary<string, Scope>();
            variables = new List<string>();
            parameters = new List<string>();
        }

        /// <summary>
        /// Adds a new child scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public Scope AddScope(string key)
        {
            if (!childScopes.ContainsKey(key))
            {
                childScopes.Add(key, new Scope(this));
                return childScopes[key];
            }
            else return null;
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Scope GetScope(string key)
        {
            if (childScopes.ContainsKey(key))
            {
                return childScopes[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds the symbol.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int AddVariable(string name)
        {
            if (variables.Contains(name) || parameters.Contains(name))
            {
                return int.MinValue;
            }
            variables.Add(name);
            return variables.Count - 1;
        }

        /// <summary>
        /// Adds the array.
        /// </summary>
        /// <param name="name">The name of the array.</param>
        /// <param name="capacity">The capacity of the array.</param>
        /// <returns></returns>
        public int AddArray(string name,int capacity)
        {
            if (variables.Contains(name) || parameters.Contains(name))
            {
                return int.MinValue;
            }
            variables.Add(name);
            for (int i = 1; i < capacity; i++)
            {
                variables.Add(name);
            }
            return variables.Count - 1;
        }

        /// <summary>
        /// does the curr scope Contains the symbol.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool ContainVariable(string name)
        {
            return variables.Contains(name) || (fatherScope == null ? false : fatherScope.ContainVariable(name));
        }

        /// <summary>
        /// Gets the stack index of variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int GetStackIndexOfVariable(string name)
        {
            int index = variables.FindIndex((str) => { return str.CompareTo(name) == 0; });

            //global scope
            if (IsGlobalScope)
            {
                return index;
            }
            else
            {
                //function scope
                if (index == -1)
                {
                    //global variable
                    return fatherScope.GetStackIndexOfVariable(name);
                }
                else
                {
                    //local variable
                    return index - variables.Count;
                }
            }
        }

        public int RemoveVariable(string name)
        {
            if (!variables.Contains(name))
            {
                return int.MinValue;
            }
            variables.Remove(name);
            return variables.Count - 1;
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int AddParameter(string name)
        {
            if (variables.Contains(name) || parameters.Contains(name))
            {
                return int.MinValue;
            }
            parameters.Add(name);
            return parameters.Count - 1;
        }

        /// <summary>
        /// does the curr scope Contains the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public bool ContainParameter(string name)
        {
            return parameters.Contains(name);
        }

        /// <summary>
        /// Gets the stack index of parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int GetStackIndexOfParameter(string name)
        {
            int index = parameters.FindIndex((str) => { return str.CompareTo(name) == 0; });
            return index - variables.Count - 1 - parameters.Count;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("variables : " + variables.Count);
            for(int i = 0; i < variables.Count; i++)
            {
                result.AppendFormat("{0} {1}", variables[i], GetStackIndexOfVariable(variables[i]));
                result.AppendLine();
            }
            result.AppendLine("parameters : " + parameters.Count);
            for (int i = 0; i < parameters.Count; i++)
            {
                result.AppendFormat("{0} {1}", parameters[i], GetStackIndexOfParameter(parameters[i]));
                result.AppendLine();
            }
            return result.ToString();
        }
    }
}
