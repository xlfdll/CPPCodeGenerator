using System;
using System.Text.RegularExpressions;

namespace CPPCodeGenerator
{
    internal static class RegexCollection
    {
        private static Regex _isCPPRegex = new Regex(@"<iostream>|using\s+namespace|using\s+std::|class\s+|public:|private:|protected:", RegexOptions.Compiled);

        private static Regex _functionRegex = new Regex(@"[a-zA-Z_0-9]*?(?:[ ]|\t)+.+?(?:[ ]|\t)*\(.*?\);", RegexOptions.Compiled);
        private static Regex _classRegex = new Regex(@"(?:class|struct)(?:[ ]|\t)+(?<ClassName>.+?)\s+{", RegexOptions.Compiled);
        private static Regex _constructorRegex = new Regex(@"^(?:[ ]|\t)*~?(?<Name>[~a-zA-Z_0-9]+?)(?:[ ]|\t)*\(.*?\);", RegexOptions.Compiled | RegexOptions.Multiline);

        internal static Regex IsCPPRegex
        {
            get { return _isCPPRegex; }
        }

        internal static Regex FunctionRegex
        {
            get { return _functionRegex; }
        }

        internal static Regex ClassRegex
        {
            get { return _classRegex; }
        }

        internal static Regex ConstructorRegex
        {
            get { return _constructorRegex; }
        }
    }
}