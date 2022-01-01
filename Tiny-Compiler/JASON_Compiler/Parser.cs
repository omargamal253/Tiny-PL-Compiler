using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JASON_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        private Node Write_Statement()
        {
            Node node = new Node("Write Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                node.Children.Add(match(Token_Class.Write));
                node.Children.Add(X_Term());
                node.Children.Add(match(Token_Class.Semicolon));

                return node;
            }

            Errors.Error_List.Add("Parsing Error: Expected " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;

        }

        private Node X_Term()
        {
            Node node = new Node("X_Term");
            Node temp = Expression();
            if (temp != null)
            {
                node.Children.Add(temp);
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Endl)
            {
                node.Children.Add(match(Token_Class.Endl));
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected--- " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                return null;
            }
            return node;
        }

        private Node Read_Statement()
        {
            Node node = new Node("Read Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                node.Children.Add(match(Token_Class.Read));
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.Semicolon));
                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;
        }

        private Node Return_Statement()
        {
            Node node = new Node("Return Statement");
            if (TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                node.Children.Add(match(Token_Class.Return));
                node.Children.Add(Expression());
                node.Children.Add(match(Token_Class.Semicolon));
                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;
        }

        private Node Condition_operation()
        {
            Node node = new Node("Condition Operation");
            if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                node.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                node.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                node.Children.Add(match(Token_Class.NotEqualOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                node.Children.Add(match(Token_Class.EqualOp));
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                return null;
            }
            return node;

        }

        private Node Condition()
        {
            Node node = new Node("Condition");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(Condition_operation());
                node.Children.Add(Term());

                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
            return null;

        }

        private Node Condition_Statement()
        {
            Node node = new Node("Condition Statement");
            Node temp = Condition();
            if (temp != null)
            {
                node.Children.Add(temp);
                node.Children.Add(F_Condition());


                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
            return null;
        }

        private Node F_Condition()
        {
            if (InputPointer == TokenStream.Count)
            {
                return null;
            }
            Node node = new Node("F Condition");
            Node temp = Boolean_operation();

            if (temp != null)
            {
                node.Children.Add(temp);
                node.Children.Add(Condition());
                node.Children.Add(F_Condition());

                return node;
            }
            return null;
        }

        private Node Boolean_operation()
        {
            Node node = new Node("Boolean Operation");
            if (TokenStream[InputPointer].token_type == Token_Class.AndOp)
            {
                node.Children.Add(match(Token_Class.AndOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.OrOP)
            {
                node.Children.Add(match(Token_Class.OrOP));
            }
            else
            {
                return null;
            }
            return node;
        }


      
    
        private Node Statements()
        {
            Node node = new Node("Statements");
            if (TokenStream[InputPointer].token_type == Token_Class.Read ||
                TokenStream[InputPointer].token_type == Token_Class.Write ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier||
                TokenStream[InputPointer].token_type == Token_Class.IF || TokenIsDataType()||
                    TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                node.Children.Add(Statement());
                node.Children.Add(StatementsDash());

                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected " +
                            TokenStream[InputPointer].token_type.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
            return null;
        }

        private Node Statement()
        {
            Node node = new Node("Statemnt");

            if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                node.Children.Add(Read_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                node.Children.Add(Write_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                InputPointer + 1 < TokenStream.Count &&
                TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
            {

                node.Children.Add(Assignment_Statement());
               

            }
            else if (TokenStream[InputPointer].token_type == Token_Class.IF)
            {
                node.Children.Add(IF_Statement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
            {
                node.Children.Add(Repeat_Statement());
            }
            else if (TokenIsDataType())
            {
                node.Children.Add(Declaration_Statement());
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                return null;
            }
            return node;
        }

        private Node StatementsDash()
        {
            Node node = new Node("Statements..");

            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Read ||
                TokenStream[InputPointer].token_type == Token_Class.Write ||
                TokenStream[InputPointer].token_type == Token_Class.Idenifier||
                TokenStream[InputPointer].token_type == Token_Class.IF || TokenIsDataType()||
                TokenStream[InputPointer].token_type == Token_Class.Repeat

                )
                {
                    node.Children.Add(Statement());
                    node.Children.Add(StatementsDash());

                    return node;
                }
            }

            return null;
        }

 
        private Node Declaration_Statement()
        {
            Node node = new Node("Declaration Statement");
            Node temp = Datatype();
            if (temp != null)
            {
                node.Children.Add(temp);
                node.Children.Add(idList());
                node.Children.Add(match(Token_Class.Semicolon));
                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected "
                        + "Datatype" + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;
        }

        private Node idList()
        {
            Node node = new Node("idList");
            Node temp = Assignment_Statement();
            if (temp != null)
            {
                node.Children.Add(temp);
                idListDash(node);
                return node;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                idListDash(node);
                return node;
            }
            Errors.Error_List.Add("Parsing Error: Expected "
                        + "Assignment Statement OR Idenifier" + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;
        }
        private void idListDash(Node node)
        {
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                match(Token_Class.Comma);
                Node Temp = Assignment_Statement();
                if (Temp != null)
                {
                    node.Children.Add(Temp);
                    idListDash(node);
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                {
                    node.Children.Add(match(Token_Class.Idenifier));
                    idListDash(node);
                }
                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                            + "Assignment Statement OR Idenifier" + " and " +
                            TokenStream[InputPointer].token_type.ToString() +
                            "  found\r\n");
                }

            }
        }
        private Node Assignment_Statement()
        {
            Node node = new Node("Assignment Statement ");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                InputPointer + 1 < TokenStream.Count &&
                TokenStream[InputPointer + 1].token_type == Token_Class.Assign)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.Assign));
                node.Children.Add(Expression());
                node.Children.Add(match(Token_Class.Semicolon));


                return node;
            }
            return null;
        }
        private Node Expression()
        {
            Node node = new Node("Expression");
            if (TokenStream[InputPointer].token_type == Token_Class.String)
            {
                node.Children.Add(match(Token_Class.String));
                return node;
            }
            Node temp = Equation();
            if (temp != null)
            {

                node.Children.Add(temp);
                return node;
            }

            temp = Term();
            if (temp != null)
            {
                node.Children.Add(temp);

                return node;
            }

            return null;
        }

        private Node Datatype()
        {
            Node node = new Node("Datatype");
            if (TokenStream[InputPointer].token_type == Token_Class.DataType_Int)
            {
                node.Children.Add(match(Token_Class.DataType_Int));
                return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DataType_Float)
            {
                node.Children.Add(match(Token_Class.DataType_Float));
                return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DataType_String)
            {
                node.Children.Add(match(Token_Class.DataType_String));
                return node;
            }

            Errors.Error_List.Add("Parsing Error: Expected "
                        + "(int Or float or string )" + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;
        }

       
        private Node Equation()
        {
            Node node = new Node("Equation");
            Node temp = Factor();
            if (temp != null)
            {
                node.Children.Add(temp);
                EquationDash(node);
                return node;
            }
            return null;
        }

        private void EquationDash(Node node)
        {
            if (InputPointer >= TokenStream.Count)
                return;
            Node temp = Arithmatic_Operator();
            if (temp == null)
                return;
            node.Children.Add(temp);
            node.Children.Add(Factor());
            EquationDash(node);
        }

        private Node Factor()
        {

            Node node = new Node("Factor");
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                node.Children.Add(match(Token_Class.LParanthesis));
                Node temp = Equation();

                if (temp == null)
                    return null;

                node.Children.Add(temp);

                node.Children.Add(match(Token_Class.RParanthesis));
                return node;
            }
            int tempPtr = InputPointer;
            if (InputPointer > 0)
                InputPointer--;
            Node prevAO = Arithmatic_Operator();
            InputPointer = tempPtr;
            Node Temp = Term();
            if (Temp != null)
            {
                Node nextAo = null;
                if (InputPointer < TokenStream.Count)
                {
                    nextAo = Arithmatic_Operator();
                }
                if (nextAo == null && prevAO == null)
                {
                    InputPointer = tempPtr;
                    return null;
                }
                if (nextAo != null)
                    InputPointer--;

                node.Children.Add(Temp);

                return node;
            }
            return null;
        }
        private Node Term()
        {
            Node node = new Node("Term");
            if (TokenStream[InputPointer].token_type == Token_Class.Number)
            {
                node.Children.Add(match(Token_Class.Number));
                return node;
            }

            Node temp = Function_call();
            if (temp != null)
            {
                node.Children.Add(temp);
                return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                return node;
            }

            Errors.Error_List.Add("Parsing Error: Expected "
                        + "Number Or Idenifier or Function call" + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
            return null;
        }

        private Node Function_call()
        {

            Node node = new Node("Function call");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier &&
                InputPointer + 1 < TokenStream.Count &&
                TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(match(Token_Class.LParanthesis));
                node.Children.Add(Arguments());
                node.Children.Add(match(Token_Class.RParanthesis));
                return node;
            }

            return null;
        }

        private Node Arguments()
        {
            Node node = new Node("Arguments");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                node.Children.Add(ArgumentsDash(node));
                return node;
            }
            return node;
        }

        private Node ArgumentsDash(Node node)
        {
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                match(Token_Class.Comma);
                node.Children.Add(match(Token_Class.Idenifier));
                ArgumentsDash(node);
            }
            return null;
        }
        Node Arithmatic_Operator()
        {
            Node node = new Node("Arithmatic Operator");
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                node.Children.Add(match(Token_Class.PlusOp));
                return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                node.Children.Add(match(Token_Class.MinusOp));
                return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                node.Children.Add(match(Token_Class.MultiplyOp));
                return node;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                node.Children.Add(match(Token_Class.DivideOp));
                return node;
            }

            return null;
        }

         private Node IF_Statement()
     {
         Node node = new Node("If Statement");
         if (TokenStream[InputPointer].token_type == Token_Class.IF)
         {
                Node n1, n2, n3, n4 , n5 ;
                n1 = match(Token_Class.IF);
                n2 = Condition_Statement();
                n3 = match(Token_Class.Then);
                n4 = Statements();
                n5 = ElsStatement();

                if (n1 != null && n2 != null && n3 != null && n4 != null && n5 != null)
                {
                     node.Children.Add(n1);
                     node.Children.Add(n2);
                     node.Children.Add(n3);
                     node.Children.Add(n4);
                     node.Children.Add(n5);

                 return node;
                }
                Errors.Error_List.Add("Parsing Error on If Statment Expected: If then Condition Statement " +
                            " and Then and Statements \r\n ");

                return null;
            }
         
         return null;
     }

          private Node ElsStatement()
   {
       Node node = new Node("ElsStatement");
       if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
       {
           node.Children.Add(Else_If_Statment());
       }
       else if (TokenStream[InputPointer].token_type == Token_Class.Else)
       {
           node.Children.Add(Else_Statment());
       }
       else if (TokenStream[InputPointer].token_type == Token_Class.End)
       {
           node.Children.Add(match(Token_Class.End));
       }
       else
       {
           Errors.Error_List.Add("Parsing Error: Expected " +
                       TokenStream[InputPointer].token_type.ToString() +
                       "  found\r\n");
           return null;
       }
       return node;
   }

        Node Else_If_Statment()
        {
            Node node = new Node("Else If_Statment");
            if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
            {

                Node n1, n2, n3, n4, n5;
                n1 = match(Token_Class.ElseIf);
                n2 = Condition_Statement();
                n3 = match(Token_Class.Then);
                n4 = Statements();
                n5 = ElsStatement();


                if (n1 != null && n2 != null && n3 != null && n4 != null && n5 != null)
                {
                    node.Children.Add(n1);
                    node.Children.Add(n2);
                    node.Children.Add(n3);
                    node.Children.Add(n4);
                    node.Children.Add(n5);

                    return node;
                }
                Errors.Error_List.Add("Parsing Error on Else If Statment Expected: ElseIf then Condition Statement " +
                            " and Then and Statements\n ");

                return null;

            }
          
            return null;
        }
        Node Else_Statment()
        {
            Node node = new Node("Else Statment");
            if (TokenStream[InputPointer].token_type == Token_Class.Else)
            {

                Node n1, n2, n3;
                n1 = match(Token_Class.Else);
                n2 = Statements();
                n3 = match(Token_Class.End);


                if (n1 != null && n2 != null && n3 != null )
                {
                    node.Children.Add(n1);
                    node.Children.Add(n2);
                    node.Children.Add(n3);

                    return node;
                }
                Errors.Error_List.Add("Parsing Error on Else Statment Expected: Else then Statements Then end\n");

                return null;

            }
           
            return null;

        }
        bool TokenIsDataType()
        {
            return (TokenStream[InputPointer].token_type == Token_Class.DataType_Int ||
                   TokenStream[InputPointer].token_type == Token_Class.DataType_Float ||
                    TokenStream[InputPointer].token_type == Token_Class.DataType_String);
        }
        Node Repeat_Statement()
        {
            Node node = new Node("Repeat Statement");

          
                if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    Node n1, n2, n3, n4;                   
                    n1 = match(Token_Class.Repeat);
                    n2 = Statements();
                    n3 =match(Token_Class.Until);
                    n4 = Condition_Statement();

                    if (n1 != null && n2 != null && n3 != null && n4 != null)
                    {
                        node.Children.Add(n1);
                        node.Children.Add(n2);
                        node.Children.Add(n3);
                        node.Children.Add(n4);

                        return node;
                    }

                Errors.Error_List.Add("Parsing Error on Repeat Statment Expected: Repeat then statments " +
                     " then until then condition statement\n ");
                return null;

           
              
                }
          
                return null;

        }

        Node FunctionName()
        {
            Node node = new Node("FunctionName");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                node.Children.Add(match(Token_Class.Idenifier));
                return node;
            }
            return null;
        }
        Node Parameter()
        {
            Node node = new Node("Parameter");
            if (TokenStream[InputPointer].token_type == Token_Class.DataType_Int ||
                TokenStream[InputPointer].token_type == Token_Class.DataType_Float ||
                TokenStream[InputPointer].token_type == Token_Class.DataType_String)
            {
                Node n1, n2 ;
                n1 = Datatype();
                n2 =match(Token_Class.Idenifier);
                if (n1 != null && n2 != null)
                {
                    node.Children.Add(n1);
                    node.Children.Add(n2);
                    return node;
                }
                return null;
            }

            return null;

        }

        Node Function_Declaration()
        {
            Node node = new Node("Function_Declaration");

            if (TokenIsDataType())
            {
                Node n1, n2, n3, n4, n5;

                n1 = Datatype();
                n2 =FunctionName();
                n3 =match(Token_Class.LParanthesis);
                n4 =F_parameter1();
                n5 =match(Token_Class.RParanthesis);

                if (n1 != null && n2 != null && n3 != null && n5 != null )
                {
                    node.Children.Add(n1);
                    node.Children.Add(n2);
                    node.Children.Add(n3);
                    node.Children.Add(n4);
                    node.Children.Add(n5);
                    return node;
                }
                Errors.Error_List.Add("Parsing Error on Function Declaration Expected: Datatype then FunctionName then  " +
                           " LParanthesis then zero or more parameter then RParanthesis \r\n ");
                return null;

            }
            return null;

        }

        private Node F_parameter1()
        {
            Node node = new Node("F_parameter1");

            if (TokenIsDataType())
            {
                node.Children.Add(Parameter());
                node.Children.Add(F_parameter2());
                return node;
            }

            return null;
        }

        private Node F_parameter2()
        {
            Node node = new Node("F_parameter2");

            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                node.Children.Add(match(Token_Class.Comma));
                node.Children.Add(Parameter());
                node.Children.Add(F_parameter2());
                return node;
            }

            return null;
        }
        Node Function_Body()
        {
            Node node = new Node("Function_Body");
            Node n1, n2, n3 , n4;

            if(TokenStream[InputPointer].token_type == Token_Class.LeftBraces)
            {
                n1 = match(Token_Class.LeftBraces);
                n2 = Statements();
                n3 = Return_Statement();
                n4 = match(Token_Class.RightBraces);

                if (n1 != null && n2 != null && n3 != null && n4 != null)
                {
                    node.Children.Add(n1);
                    node.Children.Add(n2);
                    node.Children.Add(n3);
                    node.Children.Add(n4);
                    return node;
                }
                Errors.Error_List.Add("Parsing Error on Function Body Expected: LeftBraces then Statements then Return statement then RightBraces\r\n ");
                return null;
            }
            return null;
        }
        Node Function_Statement()
        {
            //Function_Declaration
            Node node = new Node("Function_Statement");
            Node n1, n2;
            if (TokenIsDataType() && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
            {
                n1 = Function_Declaration();
                n2 = Function_Body();
                if (n1 != null && n2 != null)
                {
                    node.Children.Add(n1);
                    node.Children.Add(n2);
                    return node;
                }
                Errors.Error_List.Add("Parsing Error on Function Statement Expected: Function Declaration then Function Body \r\n ");
                return null;
            
            }
            return null;

        }

        Node Main_Function()
        {
            Node node = new Node("Main_Function");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenIsDataType())
                {

                    Node n1, n2, n3, n4, n5;

                    n1 = Datatype();
                    n2 = match(Token_Class.Main);
                    n3 = match(Token_Class.LParanthesis);
                    n4 = match(Token_Class.RParanthesis);
                    n5 = Function_Body();

                    if (n1 != null && n2 != null && n3 != null && n4 != null && n5 != null)
                    {
                        node.Children.Add(n1);
                        node.Children.Add(n2);
                        node.Children.Add(n3);
                        node.Children.Add(n4);
                        node.Children.Add(n5);
                        return node;
                    }
                    Errors.Error_List.Add("Parsing Error on  Main Function Expected: Datatype then Main then" +
                               " () then Function_Body \r\n ");
                    return null;

                }
            }
            Errors.Error_List.Add("Parsing Error:  Main Function Not found \r\n ");
            

            return null;

        }

        Node Program()
        {
            Node node = new Node("Program");

            node.Children.Add(Functions());
            node.Children.Add(Main_Function());

            return node;
        }
        Node Functions()
        {
            Node node = new Node("Functions..");

            Node n1;
            
            if (InputPointer < TokenStream.Count )
            {
                if ( TokenIsDataType() && TokenStream[InputPointer + 1].token_type != Token_Class.Main)
                {
                    node.Children.Add(Function_Statement());

                    n1 = Functions();
                    if (n1 != null) node.Children.Add(n1);

                    return node;
                }
            }

            return null;
        }
        

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    /*Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");*/
                    InputPointer++;
                    return null;
                }
            }
            else
            {
               /* Errors.Error_List.Add("Parsing Error: Expected  "
                        + ExpectedToken.ToString() + "\r\n");*/
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
