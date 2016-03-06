using System;
using System.Reflection.Emit;
using System.Reflection;

namespace ILCompilerConsole
{
    public class ILBuilder : IILBuilder
    {
        private AssemblyBuilder ThisAssemblyBuilder { get; }
        private ModuleBuilder ThisModuleBuilder { get; }
        private AssemblyName ThisAssemblyName { get; }
        private TypeBuilder DummyTypeBuilder { get; }
        private MethodBuilder MainMethodBuilder { get; }
        private ILGenerator MainMethodILGenerator { get; }

        public string CodeFilePath { get; }
        public string ExecutableFileName { get; }

        public Type DummyType { get; private set; }
        public bool Compiled { get; private set; } 

        public ILBuilder(string codeFilePath)
        {
            Compiled = false;

            CodeFilePath = codeFilePath;
            ExecutableFileName = GetExecutableFileName(codeFilePath);

            ThisAssemblyName = new AssemblyName(
                Guid.NewGuid().ToString("N"));
            ThisAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                ThisAssemblyName, AssemblyBuilderAccess.RunAndSave);
            ThisModuleBuilder = ThisAssemblyBuilder.DefineDynamicModule(
                ThisAssemblyName.Name, ExecutableFileName);
            DummyTypeBuilder = ThisModuleBuilder.DefineType(
                Constants.DummyTypeName, TypeAttributes.Public | TypeAttributes.Class  );
            MainMethodBuilder = DummyTypeBuilder.DefineMethod(
                Constants.MainMethodName, MethodAttributes.Public | MethodAttributes.Static,
                typeof(int), new Type[] { typeof(string[]) });
            MainMethodILGenerator = MainMethodBuilder.GetILGenerator();            
        }

        private string GetExecutableFileName(string codeFilePath)
        {
            var codeFileInfo = new System.IO.FileInfo(codeFilePath);
            var fileNameOnly = codeFileInfo.Name.Contains(".")
                ? codeFileInfo.Name.Split(new[] { '.' })[0]
                : codeFileInfo.Name;

            return string.Format("{0}.{1}", fileNameOnly, "exe");
        }

        public void Initialise()
        {
            Compiled = false;                    
        }

        public void GenerateExecutable()
        {
            GenerateMainMethod();

            DummyType = DummyTypeBuilder.CreateType();

            ThisAssemblyBuilder.SetEntryPoint(MainMethodBuilder, PEFileKinds.ConsoleApplication);
            ThisAssemblyBuilder.Save(ExecutableFileName);

            Compiled = true;
        }

        private void GenerateMainMethod()
        {
            EmitDiagnosticOutput(typeof(int));
            
            MainMethodILGenerator.Emit(OpCodes.Ldc_I4_0);
            MainMethodILGenerator.Emit(OpCodes.Ret);            
        }

        private void EmitHelloWorld()
        {
            MainMethodILGenerator.Emit(OpCodes.Ldstr, "Hello world!");
            MainMethodILGenerator.Emit(OpCodes.Call,
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
        }

        private void EmitDiagnosticOutput(Type type)
        {            
            MainMethodILGenerator.Emit(OpCodes.Call,
                typeof(Console).GetMethod("WriteLine", new Type[] { type }));
        }

        /////////////////////////////////////
        public void EmitPushIntegerOnStack(char charNum)
        {
            int intNum = (int) Char.GetNumericValue(charNum);
            MainMethodILGenerator.Emit(OpCodes.Ldc_I4, intNum);
        }

        public void EmitAdd()
        {
            MainMethodILGenerator.Emit(OpCodes.Add);
        }
    }
}
