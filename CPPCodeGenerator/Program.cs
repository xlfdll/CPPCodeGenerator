using System;
using System.Text;
using System.Windows.Forms;
using System.IO;

using CPPCodeGenerator.Properties;

namespace CPPCodeGenerator
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            String headerFileName = String.Empty;
            String codeFileName = String.Empty;

            Boolean isOverwriteOn = false; // Switch "/overwrite" boolean value

            // CPPCodeGenerator /?
            if (args.Length == 1 && args[0] == Resources.HelpSwitchName)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(Application.ProductName);
                sb.AppendLine(String.Format("Version {0}", Application.ProductVersion.ToString()));
                sb.AppendLine("© 2011 Xlfdll Workstation");
                sb.AppendLine();
                sb.AppendLine("Create a C/C++ code file with proper extension containing corresponding function bodies, according to the contents of the input header file");
                sb.AppendLine();
                sb.AppendLine("Usage:");
                sb.AppendLine();
                sb.AppendLine("CPPCodeGenerator [<HeaderFilePath> [<CodeFilePath>]] [/overwrite]");
                sb.AppendLine();
                sb.AppendLine("Parameters:");
                sb.AppendLine();
                sb.AppendLine("<HeaderFilePath> - Specifies the path of the header file for the input");
                sb.AppendLine("<CodeFilePath> - Specifies the path of the code file for the output");
                sb.AppendLine();
                sb.AppendLine("/overwrite - Suppresses the prompt and overwrites the code file automatically");

                MessageBox.Show(sb.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            // CPPCodeGenerator [/overwrite]
            if (args.Length == 0 || (args.Length == 1 && args[0] == Resources.OverwriteSwitchName))
            {
                using (OpenFileDialog headerOpenFileDialog = new OpenFileDialog())
                {
                    headerOpenFileDialog.Filter = "C / C++ Header file (*.h)|*.h|All Files (*.*)|*.*";
                    headerOpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                    if (headerOpenFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        headerFileName = headerOpenFileDialog.FileName;

                        Boolean isCPP = Helper.ContainCPPHeaderContents(File.ReadAllText(headerFileName));

                        using (SaveFileDialog codeSaveFileDialog = new SaveFileDialog())
                        {
                            codeSaveFileDialog.Filter = String.Concat(isCPP ? "C++ Code file (*.cpp)|*.cpp|" : "C Code file (*.c)|*.c|", "All files (*.*)|*.*");
                            codeSaveFileDialog.InitialDirectory = Path.GetDirectoryName(headerOpenFileDialog.FileName);
                            codeSaveFileDialog.FileName = Path.GetFileNameWithoutExtension(headerFileName);
                            codeSaveFileDialog.OverwritePrompt = !(args.Length == 1 && args[0] == Resources.OverwriteSwitchName);

                            if (codeSaveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                codeFileName = codeSaveFileDialog.FileName;
                            }
                        }
                    }
                }

                isOverwriteOn = true;
            }
            // CPPCodeGenerator <HeaderFilePath> [/overwrite]
            else if (args.Length == 1 || (args.Length == 2 && args[1] == Resources.OverwriteSwitchName))
            {
                if (File.Exists(headerFileName))
                {
                    headerFileName = args[0];

                    Boolean isCPP = Helper.ContainCPPHeaderContents(File.ReadAllText(headerFileName));

                    codeFileName = Path.Combine(Path.GetDirectoryName(headerFileName), String.Concat(Path.GetFileNameWithoutExtension(headerFileName), (isCPP ? ".cpp" : ".c")));

                    isOverwriteOn = (args.Length == 2 && args[1] == Resources.OverwriteSwitchName);
                }
                else
                {
                    MessageBox.Show(String.Format("The following file does not exist:{0}{0}{1}", Environment.NewLine, args[0]), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // CPPCodeGenerator <HeaderFilePath> <CodeFilePath> [/overwrite]
            else
            {
                if (File.Exists(headerFileName))
                {
                    headerFileName = args[0];
                    codeFileName = args[1];

                    isOverwriteOn = (args[args.Length - 1] == Resources.OverwriteSwitchName);
                }
                else
                {
                    MessageBox.Show(String.Format("The following file does not exist:{0}{0}{1}", Environment.NewLine, args[0]), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // If the code file name is empty, the whole process must be stopped
            // If the code file name is not empty, the header file name must not be empty

            if (!String.IsNullOrEmpty(codeFileName) && !isOverwriteOn)
            {
                DialogResult isFileExistDialogResult = MessageBox.Show(String.Format("File {0} already exists.{1}{1}Do you want to overwrite?", Path.GetFileName(codeFileName), Environment.NewLine), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (isFileExistDialogResult != DialogResult.Yes)
                {
                    return; // Application.Exit() cannot be used here. It will not exit the program immediately.
                }
            }

            if (File.Exists(headerFileName) && !String.IsNullOrEmpty(codeFileName))
            {
                try
                {
                    Helper.WriteCodeContents(headerFileName, codeFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("The following exception has occurred:{0}{0}{1}", Environment.NewLine, ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}