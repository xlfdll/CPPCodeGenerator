using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CPPCodeGenerator
{
    internal static class Helper
    {
        internal static Boolean ContainCPPHeaderContents(String headerContents)
        {
            return RegexCollection.IsCPPRegex.IsMatch(headerContents);
        }

        internal static String GetCCodeContents(String headerContents)
        {
            String result = String.Empty;

            MatchCollection functionMatchCollection = RegexCollection.FunctionRegex.Matches(headerContents);

            if (functionMatchCollection.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (Int32 i = 0; i < functionMatchCollection.Count; i++)
                {
                    sb.AppendLine(functionMatchCollection[i].Value.TrimEnd(';'));
                    sb.AppendLine("{");
                    sb.AppendLine("\t");
                    sb.AppendLine("}");

                    if (i != functionMatchCollection.Count - 1)
                    {
                        sb.AppendLine();
                    }
                }

                result = sb.ToString();
            }

            return result;
        }

        internal static String GetCPPCodeContents(String headerContents)
        {
            String result = String.Empty;

            MatchCollection functionMatchCollection = RegexCollection.FunctionRegex.Matches(headerContents);

            if (functionMatchCollection.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                for (Int32 i = 0; i < functionMatchCollection.Count; i++)
                {
                    Match constructorMatch = RegexCollection.ConstructorRegex.Match(functionMatchCollection[i].Value);

                    if (constructorMatch.Success)
                    {
                        sb.AppendFormat("{0}::{1}", constructorMatch.Groups["Name"].Value, constructorMatch.Value.Trim(';', '\t'));
                        sb.AppendLine();
                    }
                    else
                    {
                        String[] functionTokens = functionMatchCollection[i].Value.TrimEnd(';').Split(new Char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        Int32 functionIndex = headerContents.IndexOf(functionMatchCollection[i].Value);
                        Int32 classStartIndex = headerContents.LastIndexOf('}', functionIndex);

                        MatchCollection classMatchCollection = RegexCollection.ClassRegex.Matches(headerContents.Substring(classStartIndex == -1 ? 0 : classStartIndex, functionIndex - classStartIndex));

                        sb.Append(functionTokens[0]);
                        sb.Append(' ');

                        foreach (Match match in classMatchCollection)
                        {
                            sb.AppendFormat("{0}::", match.Groups["ClassName"].Value);
                        }

                        for (Int32 j = 1; j < functionTokens.Length; j++)
                        {
                            sb.Append(functionTokens[j]);

                            if (j != functionTokens.Length - 1)
                            {
                                sb.Append(' ');
                            }
                        }

                        sb.AppendLine();
                    }

                    sb.AppendLine("{");
                    sb.AppendLine("\t");
                    sb.AppendLine("}");

                    if (i != functionMatchCollection.Count - 1)
                    {
                        sb.AppendLine();
                    }
                }

                result = sb.ToString();
            }

            return result;
        }

        internal static void WriteCodeContents(String headerFileName, String codeFileName)
        {
            String headerContents = File.ReadAllText(headerFileName);

            // Use header file contents to determine if it is C++, instead of filename, to allow customization
            Boolean isCPP = ContainCPPHeaderContents(headerContents);

            File.WriteAllText(codeFileName, 
                String.Concat(String.Format("#include \"{0}\"{1}{1}", Path.GetFileName(headerFileName), Environment.NewLine),
                isCPP ? Helper.GetCPPCodeContents(headerContents) : Helper.GetCCodeContents(headerContents)));
        }
    }
}