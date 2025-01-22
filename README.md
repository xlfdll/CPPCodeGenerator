# C / C++ Code File Generator
A simple program to create C / C++ skeleton code files with empty function definitions, based on existing header files

## System Requirements
* .NET Framework 2.0

[Runtime configuration](https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-configure-an-app-to-support-net-framework-4-or-4-5) is needed for running on .NET Framework 4.0+.

## Usage
```
CPPCodeGenerator [/?] | [<HeaderFilePath> [<CodeFilePath>]] [/overwrite]
```
* **\<no arguments\>** - Interactive mode (file open and save dialogs will open)
* **/?** - Show usage
* **\<HeaderFilePath\> \[\<CodeFilePath\>\]** - Silent mode. Create new code file based on given header file path
  * \<CodeFilePath\> is optional. If not specified, the new code file will have the same file name as header file's
* **/overwrite** - In both interactive and silent modes, suppress the prompt and overwrite code file automatically

## Development Prerequisites
* Visual Studio 2012+

Before the build, generate-build-number.sh needs to be executed in a Git / Bash shell to generate build information code file (`BuildInfo.cs`).
