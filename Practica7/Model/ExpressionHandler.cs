using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using PracticalWork7.ViewModel.Helpers;

namespace PracticalWork7.Model
{
    internal class ExpressionHandler : BindingHelper
    {
        private static ExpressionHandler instance = new ExpressionHandler();
        private string _text;
        private DataTable _table;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public DataTable Table
        {
            get { return _table; }
            set
            {
                _table = value;
                OnPropertyChanged();
            }
        }

        public DataTable SetTable()
        {
            string expression = Text.Replace(" ", "");
            List<string> variables = GetVariables(expression);
            if (variables.Count < 2)
            {
                return null;
            }
            string expression2 = ConvertExpressionToRPN(expression);
            Table = GenerateTruthTable(variables, expression2);
            SaveInstance(Text, Table);
            return Table;
        }

        public void SaveInstance(string text, DataTable dataTable)
        {
            instance.Text = text;
            instance.Table = dataTable;
        }

        public ExpressionHandler GetInstance()
        {
            return instance;
        }

        private List<string> GetVariables(string expression) // Получаем все переменные из выражения
        {
            List<string> variables = new List<string>();

            IEnumerable<char> chars = expression.Distinct();

            foreach (char c in chars)
            {
                if (char.IsLetter(c))
                {
                    variables.Add(c.ToString());
                }
            }

            return variables;
        }

        private string ConvertExpressionToRPN(string expression) // Преобразуем выражение в форму обратной польской нотации
        {
            char[] tokens = TokenizeExpression(expression);
            List<char> outputList = new List<char>();
            Stack<char> operatorStack = new Stack<char>();

            foreach (char token in tokens)
            {
                if (IsOperand(token))
                {
                    outputList.Add(token);
                }
                else if (IsPrefixFunction(token))
                {
                    operatorStack.Push(token);
                }
                else if (token == '(')
                {
                    operatorStack.Push(token);
                }
                else if (token == ')')
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                    {
                        outputList.Add(operatorStack.Pop());
                    }

                    operatorStack.Pop();

                    if (operatorStack.Count > 0 && IsPrefixFunction(operatorStack.Peek()))
                    {
                        outputList.Add(operatorStack.Pop());
                    }
                }
                else if (IsBinaryOperator(token))
                {
                    while (operatorStack.Count > 0 &&
                           (IsPrefixFunction(operatorStack.Peek()) ||
                            OperatorPrecedence(token) <= OperatorPrecedence(operatorStack.Peek()) ||
                            (IsLeftAssociative(token) && OperatorPrecedence(token) == OperatorPrecedence(operatorStack.Peek()))))
                    {
                        outputList.Add(operatorStack.Pop());
                    }

                    operatorStack.Push(token);
                }
            }

            while (operatorStack.Count > 0)
            {
                outputList.Add(operatorStack.Pop());
            }

            return string.Join(" ", outputList);
        }

        private char[] TokenizeExpression(string expression)
        {
            List<char> tokens = new List<char>();
            int i = 0;

            while (i < expression.Length)
            {
                if (IsOperator(expression[i]))
                {
                    tokens.Add(expression[i]);
                    i++;
                }
                else if (IsOperand(expression[i]))
                {
                    char operand = expression[i];

                    while (i + 1 < expression.Length && IsOperand(expression[i + 1]))
                    {
                        operand += expression[i + 1];
                        i++;
                    }

                    tokens.Add(operand);
                    i++;
                }
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    tokens.Add(expression[i]);
                    i++;
                }
            }

            return tokens.ToArray();
        }

        private bool IsOperand(char token)
        {
            return !IsOperator(token) && token != '(' && token != ')' && char.IsLetter(token);
        }

        private bool IsOperator(char token)
        {
            return token == '!' || token == '∧' || token == '∨' || token == '⊕' ||
                   token == '↑' || token == '↓' || token == '≡' || token == '→';
        }

        private bool IsPrefixFunction(char token)
        {
            return token == '!';
        }

        private bool IsBinaryOperator(char token)
        {
            return token == '∧' || token == '∨' || token == '⊕' || token == '↑' ||
                   token == '↓' || token == '≡' || token == '→';
        }

        private int OperatorPrecedence(char token)
        {
            switch (token)
            {
                case '!':
                    return 4;
                case '∧':
                    return 3;
                case '∨':
                    return 2;
                case '⊕':
                case '↑':
                case '↓':
                case '≡':
                case '→':
                    return 1;
                default:
                    return 0;
            }
        }

        private bool IsLeftAssociative(char token)
        {
            return true; // Потому что все левоассоциативные =)
        }

        private DataTable GenerateTruthTable(List<string> variables, string expression) // Создаём таблицу истинности из выражения
        {
            int rowCount = (int)Math.Pow(2, variables.Count);
            DataTable truthTable = new DataTable();

            foreach (string variable in variables)
            {
                truthTable.Columns.Add(variable);
            }

            truthTable.Columns.Add(expression, typeof(bool));

            List<List<bool>> variableValues = new List<List<bool>>();

            for (int i = 0; i < rowCount; i++)
            {
                List<bool> values = new List<bool>();

                string binary = Convert.ToString(i, 2).PadLeft(variables.Count, '0');

                foreach (char bit in binary)
                {
                    bool value = bit == '1';
                    values.Add(value);
                }

                variableValues.Add(values);
            }

            foreach (List<bool> values in variableValues)
            {
                DataRow row = truthTable.NewRow();

                for (int i = 0; i < variables.Count; i++)
                {
                    row[i] = values[i];
                }

                bool result = EvaluateExpression(expression, values, variables);
                row[expression] = result;

                truthTable.Rows.Add(row);
            }

            return truthTable;
        }

        private bool EvaluateExpression(string expression, List<bool> variableValues, List<string> variables)
        {
            Stack<bool> stack = new Stack<bool>();

            foreach (char token in expression)
            {
                if (IsOperand(token))
                {
                    int variableIndex = variables.IndexOf(token.ToString());
                    bool value = variableValues[variableIndex];
                    stack.Push(value);
                }
                else if (IsOperator(token))
                {
                    if (token == '!')
                    {
                        bool operand = stack.Pop();
                        bool result = ApplyOperator(token, operand);
                        stack.Push(result);
                    }
                    else
                    {
                        bool operand2 = stack.Pop();
                        bool operand1 = stack.Pop();
                        bool result = ApplyOperator(token, operand1, operand2);
                        stack.Push(result);
                    }
                }
            }

            return stack.Pop();
        }

        private bool ApplyOperator(char token, bool operand1, bool operand2 = false)
        {
            switch (token)
            {
                case '∧':
                    return operand1 && operand2;
                case '∨':
                    return operand1 || operand2;
                case '⊕':
                    return operand1 ^ operand2;
                case '↑':
                    return !operand1 && operand2;
                case '↓':
                    return !operand1 || operand2;
                case '≡':
                    return operand1 == operand2;
                case '→':
                    return !operand1 || operand2;
                case '!':
                    return !operand1;
                default:
                    throw new ArgumentException("Invalid operator: " + token);
            }
        }
    }
}
