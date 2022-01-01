using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

public enum Token_Class
{
    Else, Until, IF, DataType_Int, DataType_Float, DataType_String, LeftBraces, RightBraces,
    Read, Then, While, Write, Repeat, ElseIf, Return, Endl, Dot, String,
    Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, PlusOp, MinusOp, MultiplyOp, DivideOp, OrOP, AndOp,
    Idenifier, Number, Assign, Comment, NotEqualOp, booleanOP, End, Main
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.IF);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("int", Token_Class.DataType_Int);
            ReservedWords.Add("float", Token_Class.DataType_Float);
            ReservedWords.Add("string", Token_Class.DataType_String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("while", Token_Class.While);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);
            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("{", Token_Class.LeftBraces);
            Operators.Add("}", Token_Class.RightBraces);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add("||", Token_Class.OrOP);
            Operators.Add("<>", Token_Class.NotEqualOp);



        }
        public bool is_letter(char x)
        {
            if (x >= 'a' && x <= 'z')
                return true;
            if (x >= 'A' && x <= 'Z')
                return true;
            return false;

        }
        public bool is_digit(char n)
        {
            return (n >= '0' && n <= '9');
        }
        public bool is_space(char n)
        {
            return (n == ' ' || n == '\n' || n == '\r');
        }
        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                // string CurrentLexeme = CurrentChar.ToString();
                string CurrentLexeme = "";

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t')
                    continue;

                if (is_letter(CurrentChar)) //if you read a character
                {
                    while (j < SourceCode.Length && (is_letter(SourceCode[j]) || is_digit(SourceCode[j])))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }

                else if (is_digit(CurrentChar))
                {
                    while (j < SourceCode.Length && (is_digit(SourceCode[j]) || SourceCode[j] == '.'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;


                }
                else if (CurrentChar == '|')
                {
                    while (j < SourceCode.Length && SourceCode[j] == '|')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == '&')
                {
                    while (j < SourceCode.Length && SourceCode[j] == '&')
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }
                else if (CurrentChar == '<')
                {

                    while (j < SourceCode.Length && (SourceCode[j] == '<' || SourceCode[j] == '>'))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }
                else if (CurrentChar == ':')
                {

                    while (j < SourceCode.Length && (SourceCode[j] == ':' || SourceCode[j] == '='))
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }
                else if (CurrentChar == '"')
                {
                    while (j < SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                        if (j == SourceCode.Length)
                        {
                            break;
                        }
                        if (SourceCode[j] == '"')
                        {
                            CurrentLexeme += SourceCode[j];
                            break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '/' && SourceCode[i + 1] == '*')
                {
                    while (j <= SourceCode.Length)
                    {
                        CurrentLexeme += SourceCode[j];
                        j++;
                        if (j == SourceCode.Length)
                        {
                            break;
                        }

                        if (SourceCode[j] == '*' && SourceCode[j + 1] == '/')
                        {
                            CurrentLexeme += "*/";
                            j++;
                            break;
                        }
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }

                else
                {
                    CurrentLexeme += SourceCode[j];
                    FindTokenClass(CurrentLexeme);
                }
            }


            /*using (StringReader sr = new StringReader(SourceCode))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string [] arrLine = line.Split('k');
                    MessageBox.Show(line);
                }
            }*/

            JASON_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            // Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
                return;
            }


            //Is it an identifier?

            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
                return;
            }
            // Is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
                return;
            }
            //Is it a Constant?
            else if (isConstant(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
                return;
            }

            //Is it an operator?
            else if (Operators.ContainsKey(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
                return;
            }
            //Is it a comment?
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
                return;
            }

            //Is it an undefined?
            Errors.Error_List.Add("Unrecognized token : " + Lex);
        }

        bool isComment(string lex)
        {
            // Check if the lex is a multi-line comment or not.

            Regex reg = new Regex(@"^/\*(.)*\*/$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }
        bool isString(string lex)
        {
            // Check if the lex is a string or not.

            Regex reg = new Regex(@"^""(.)*""$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }

        bool isIdentifier(string lex)
        {
            // Check if the lex is an identifier or not.

            Regex reg = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9])*$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }
        bool isConstant(string lex)
        {

            // Check if the lex is a constant (Number) or not.
            Regex reg = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }

        //bool isbooleanOP(string lex)
        //{
        //    //bool isValid = true;
        //    // Check if the lex is a constant (booleanOP) or not.
        //    Regex reg = new Regex(@"^(&& | \|\|)$", RegexOptions.Compiled);
        //    return reg.IsMatch(lex);
        //}



    }
}
