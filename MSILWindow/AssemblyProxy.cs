using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MSILWindow
{
    public class AssemblyProxy : MarshalByRefObject
    {
        public string LoadInfoFromAssembly(string path, string className, string methodName)
        {
            var result = "";
            var assembly = Assembly.LoadFile(path);

            var type = assembly.GetTypes().FirstOrDefault(x => x.Name.ToUpper().Equals(className.ToUpper()));

            MethodInfo method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).FirstOrDefault(x => x.Name.ToUpper().Equals(methodName.ToUpper()));
            if (method == null)
            {
                result = "No method found";
                return result;
            }

            foreach (var instruciton in FindLiterals(method))
            {
                result += instruciton + Environment.NewLine;
            }

            return result;
        }

        private static IEnumerable<string> FindLiterals(MethodInfo method)
        {
            ILReader reader = new ILReader(method);
            foreach (ILInstruction instruction in reader.Instructions)
            {
                yield return instruction.ToString() as string;
            }
        }
    }
}