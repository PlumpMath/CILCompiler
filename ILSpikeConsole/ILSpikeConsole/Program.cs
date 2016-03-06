using System.Reflection;

namespace ILCompilerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string codeFilePath = args.Length> 0
                ? args[0] : "DefaultApp.p";

            //compile input code into CIL
            IILBuilder ilBulder = new ILBuilder(codeFilePath);
            ilBulder.Initialise();
            ICodeParser codeParser = new CodeParser(codeFilePath, ilBulder);            
            codeParser.Initialise();
            codeParser.ParseCode();

            //generate executable
            ilBulder.GenerateExecutable();

            //execute compiled code within this console
            ilBulder.DummyType
                .GetMethod("Main", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { new string[] { } });
        }
    }
}
