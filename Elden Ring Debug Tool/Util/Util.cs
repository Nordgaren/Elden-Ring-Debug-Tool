using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Elden_Ring_Debug_Tool
{
    internal class Util
    {
        public static int DeleteFromEnd(int num, int n)
        {
            for (int i = 1; num != 0; i++)
            {
                num = num / 10;

                if (i == n)
                    return num;
            }

            return 0;
        }

        public static string GetResource(string item)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Elden_Ring_Debug_Tool.Resources.{item}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
