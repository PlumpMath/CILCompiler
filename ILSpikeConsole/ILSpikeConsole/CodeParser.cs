using System;
using System.IO;

namespace ILCompilerConsole
{
    public class CodeParser : ICodeParser
    {
        private string CodeFilePath { get; }
        private char Lookahead { get; set; }
        private TextReader CodeFileReader { get; set; }
        private IILBuilder ILBuilder { get; }

        public CodeParser(string codeFilePath, IILBuilder ilBuider)
        {
            CodeFilePath = codeFilePath;
            ILBuilder = ilBuider;
        }

        public void Initialise()
        {
            var codeFileInfo = new FileInfo(CodeFilePath);
            CodeFileReader = codeFileInfo.OpenText();
        }

        public void ParseCode()
        {
            InitParsing();
            Expression();

            CodeFileReader.Close();
        }

        ///////////////////////////////////
        private void InitParsing()
        {
            GetChar();
        }

        ///////////////////////////////////

        private void Expression()
        {
            ILBuilder.EmitPushIntegerOnStack(GetNum());

        }

        ///////////////////////////////////

        private void Match(char x)
        {
            if (Lookahead == x)
                GetChar();
            else
                Expected(string.Format("'{0}'", x));
        }

        /// <summary>
        /// Get an identifier
        /// </summary>
        /// <returns></returns>
        private char GetName()
        {
            if (!IsAlpha(Lookahead)) Expected("Name");
            var name = char.ToUpper(Lookahead);
            GetChar();
            return name;
        }

        /// <summary>
        /// Get a number
        /// </summary>
        /// <returns></returns>
        private char GetNum()
        {
            if (!IsDigit(Lookahead)) Expected("Integer");
            var num = Lookahead;
            GetChar();
            return num;
        }

        ///////////////////////////////////
        private void GetChar()
        {
            Lookahead = (char)CodeFileReader.Read();
        }

        //////////////////////////////////
        private void Expected(string s)
        {
            Abort(string.Format("{0} Expected", s));
        }

        private void Abort(string errorMsg)
        {
            Error(errorMsg);
            Environment.Exit(0);
        }

        private void Error(string s)
        {
            Console.WriteLine();
            Console.WriteLine("Error: {0}.", s);
        }

        //////////////////////////////////
        private bool IsAlpha(char c)
        {
            return char.IsLetter(c);
        }

        private bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }
    }
}