using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Calcul_simply_xamarin
{
    public partial class MainPage : ContentPage
    {
        Label label_text;
        public MainPage()
        {
            InitializeComponent();

        }

        private Button Generate_Button(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.BorderColor = Color.Black;
            button.Clicked += Set_Number;
            return button;
        }

        private Button Clear_Button(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Clicked += (sender, e) =>
            {
                label_text.Text = "";
            };
            return button;
        }

        private Button Delete_char(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Clicked += (sender, e) =>
            {
                if (label_text.Text.Length > 1)
                    label_text.Text = label_text.Text.Substring(0, text.Length - 1);
                else
                    label_text.Text = "";
            };
            return button;
        }

        private Button Button_Task(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.BorderColor = Color.Green;
            button.Clicked += Get_Answer;
            return button;
        }
        protected override void OnAppearing()
        {
            StackLayout stack_main = new StackLayout();
            stack_main.Margin = 12;

            label_text = new Label();
            label_text.FontSize = 24;
            stack_main.Children.Add(label_text);

            Grid grid = new Grid();              
            grid.Children.Add(Generate_Button("+"), 0, 0);
            grid.Children.Add(Generate_Button("1"), 0, 1);
            grid.Children.Add(Generate_Button("2"), 0, 2);
            grid.Children.Add(Generate_Button("3"), 0, 3);
            grid.Children.Add(Generate_Button("("), 0, 4);
            grid.Children.Add(Generate_Button("-"), 1, 0);
            grid.Children.Add(Generate_Button("4"), 1, 1);
            grid.Children.Add(Generate_Button("5"), 1, 2);
            grid.Children.Add(Generate_Button("6"), 1, 3);
            grid.Children.Add(Generate_Button("0"), 1, 4);
            grid.Children.Add(Generate_Button("*"), 2, 0);
            grid.Children.Add(Generate_Button("7"), 2, 1);
            grid.Children.Add(Generate_Button("8"), 2, 2);
            grid.Children.Add(Generate_Button("9"), 2, 3);
            grid.Children.Add(Generate_Button(")"), 2, 4);
            grid.Children.Add(Generate_Button("/"), 3, 0);
            grid.Children.Add(Generate_Button("."), 3, 1);
            grid.Children.Add(Button_Task("="), 3, 2);
            grid.Children.Add(Delete_char("<-"), 3, 3);
            grid.Children.Add(Clear_Button("C"), 3, 4);

            stack_main.Children.Add(grid);
            Content = stack_main;
        }

        private void Set_Number(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            label_text.Text = label_text.Text + button.Text;
        }

        private void Get_Answer(object sender, EventArgs e)
        {
            string answer = label_text.Text;
            if (answer == null)
                return;
            try
            {
                double result =  Solve(answer);
                label_text.Text = label_text.Text + "= " + Convert.ToString(result);
                 
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public static double Solve(string equation)
        {
            if (!IsValidEquation(equation))
            {
                throw new ArgumentException("Неверное уравнение");
            }

            // Преобразуем строку в список токенов
            List<string> tokens = Tokenize(equation);

            // Вычисляем выражение
            return Evaluate(tokens);
        }

        private static bool IsValidEquation(string equation)
        {
            int openBracketCount = 0;
            foreach (char c in equation)
            {
                if (c == '(')
                {
                    openBracketCount++;
                }
                else if (c == ')')
                {
                    openBracketCount--;
                    if (openBracketCount < 0)
                    {
                        return false; // Неверный баланс скобок
                    }
                }
            }
            return openBracketCount == 0; // Все скобки закрыты
        }

        private static List<string> Tokenize(string equation)
        {
            List<string> tokens = new List<string>();
            string currentToken = "";
            foreach (char c in equation)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    currentToken += c;
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')')
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    tokens.Add(c.ToString());
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (currentToken != "")
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                }
            }
            if (currentToken != "")
            {
                tokens.Add(currentToken);
            }
            Console.WriteLine(tokens.ToArray());
            return tokens;
        }


        private static double Evaluate(List<string> tokens)
        {
            // Обработка скобок
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "(")
                {
                    int closingBracketIndex = FindMatchingBracket(tokens, i);
                    List<string> subExpression = tokens.GetRange(i + 1, closingBracketIndex - i - 1);
                    double result = Evaluate(subExpression);
                    tokens.RemoveRange(i, closingBracketIndex - i + 1);
                    tokens.Insert(i, result.ToString());
                    i--;
                }
            }

            // Обработка умножения и деления
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] == "*" || tokens[i] == "/")
                {
                    double leftOperand = double.Parse(tokens[i - 1]);
                    double rightOperand = double.Parse(tokens[i + 1]);
                    double result = tokens[i] == "*" ? leftOperand * rightOperand : leftOperand / rightOperand;
                    tokens.RemoveRange(i - 1, 3);
                    tokens.Insert(i - 1, result.ToString());
                    i--;
                }
            }

            // Обработка сложения и вычитания
            double resultValue = double.Parse(tokens[0]);
            for (int i = 1; i < tokens.Count; i += 2)
            {
                double operand = double.Parse(tokens[i + 1]);
                if (tokens[i] == "+")
                {
                    resultValue += operand;
                }
                else if (tokens[i] == "-")
                {
                    resultValue -= operand;
                }
            }
            return resultValue;
        }


        private static int FindMatchingBracket(List<string> tokens, int openingBracketIndex)
        {
            int openBracketCount = 1;
            for (int i = openingBracketIndex + 1; i < tokens.Count; i++)
            {
                if (tokens[i] == "(")
                {
                    openBracketCount++;
                }
                else if (tokens[i] == ")")
                {
                    openBracketCount--;
                    if (openBracketCount == 0)
                    {
                        return i;
                    }
                }
            }
            return -1; // Не найден соответствующий закрывающий символ
        }


    }

}
