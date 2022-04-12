using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Elden_Ring_Debug_Tool
{
    class GetTxtResourceClass
    {
        public static readonly string ExeDir = Environment.CurrentDirectory;

        public static string GetTxtResource(string filePath)
        {
            //Get local directory + file path, read file, return string contents of file

            //Path.Combine(Environment.CurrentDirectory, filePath);

            string fileString = File.ReadAllText($@"{ExeDir}/{filePath}");

            return fileString;
        }

        public static bool IsValidTxtResource(string txtLine)
        {
            //see if txt resource line is valid and should be accepted 
            //(bare bones, only checks for a couple obvious things)

            if (txtLine.Contains("//"))
            {
                txtLine = txtLine.Substring(0, txtLine.IndexOf("//")); // remove everything after "//" comments
            };
            if (string.IsNullOrWhiteSpace(txtLine) == true) //empty line check
            {
                return false; //resource line invalid
            };

            return true; //resource line valid
        }

        public static string[] RegexSplit(string source, string pattern)
        {
            return Regex.Split(source, pattern);
        }
    }
}