using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Calculator.Commands;
using Calculator.Models;
using Calculator.Views;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : ViewModelBase
    {

        #region Приватные поля

        private CalculationModel calculation;

        private DelegateCommand<string> digitButtonPressCommand;
        private DelegateCommand<string> operationButtonPressCommand;

        private bool newDisplayRequired = false;
        private string fullExpression;
        private string display;
        private int leftBreackets = 0;

        #endregion

        #region Конструктор

        public CalculatorViewModel()
        {
            calculation = new CalculationModel();
            display = string.Empty;
            fullExpression = string.Empty;
            LastOperation = String.Empty;
        }

        #endregion

        #region Публичные свойства

        public string LastOperation { get; set; }

        public string InputExpression
        {
            get { return calculation.InputExpression; }
            set { calculation.InputExpression = value; }
        }

        public string PostfixExpr
        {
            get { return calculation.PostfixExpr; }
        }

        public string Result
        {
            get { return calculation.Result; }
        }

        public string Display
        {
            get { return display; }
            set
            {
                display = value;
                OnPropertyChanged("Display");
            }
        }

        public string FullExpression
        {
            get { return fullExpression; }
            set
            {
                fullExpression = value;
                OnPropertyChanged("FullExpression");
            }
        }

        #endregion

        #region Commands

        public ICommand OperationButtonPressCommand
        {
            get
            {
                if (operationButtonPressCommand == null)
                {
                    operationButtonPressCommand = new DelegateCommand<string>(
                        OperationButtonPress, CanOperationButtonPress);
                }
                return operationButtonPressCommand;
            }
        }

        private static bool CanOperationButtonPress(string number)
        {
            return true;
        }

        public ICommand DigitButtonPressCommand
        {
            get
            {
                if (digitButtonPressCommand == null)
                {
                    digitButtonPressCommand = new DelegateCommand<string>(
                        DigitButtonPress, CanDigitButtonPress);
                }
                return digitButtonPressCommand;
            }
        }

        private static bool CanDigitButtonPress(string button)
        {
            return true;
        }

        public void DigitButtonPress(string button)
        {
            switch (button)
            {
                case "C":
                    Display = string.Empty;
                    InputExpression = string.Empty;
                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    leftBreackets = 0;
                    break;
                case "Del":
                    if (display.Length > 1)
                    {
                        if (Display.Last() == ')') leftBreackets += 1;
                        else if (Display.Last() == '(')
                        {
                            if (leftBreackets > 0) leftBreackets -= 1;
                        }
                        Display = display.Substring(0, display.Length - 1);
                    }
                    else Display = string.Empty;

                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    break;
                case "+/-":
                    if (display != "")
                        if (display.Contains("-"))
                        {
                            Display = display.Remove(display.IndexOf("-"), 1);
                        }
                        else Display = "-" + display;
                    break;
                case ",":
                    if (newDisplayRequired || display == "")
                    {
                        Display = "0,";
                    }
                    else if (!display.Contains("."))
                    {
                        Display = display + ",";
                    }
                    break;
                default:
                    if (display == "" || newDisplayRequired)
                    {
                        Display = button;
                        FullExpression = "";
                    }
                    else
                        Display = display + button;
                    LastOperation = string.Empty;
                    break;
            }
            newDisplayRequired = false;
        }

        public void OperationButtonPress(string operation)
        {
            try
            {
                switch (operation)
                {
                    case "(":
                        if (display == "" || (!Char.IsDigit(Display.Last()) && Display.Last() != ')'))
                        {
                            leftBreackets += 1;
                            Display = display + "(";
                            LastOperation = string.Empty;
                        }
                        break;
                    case ")":
                        if (leftBreackets > 0 && (Char.IsDigit(Display.Last()) || Display.Last() == ')'))
                        {
                            leftBreackets -= 1;
                            Display = display + ")";
                        }
                        break;
                }

                if (LastOperation == string.Empty && Display.Last() != '(' || operation == "Sqrt")
                {
                    switch (operation)
                    {
                        case "/":
                            Display = display + "/";
                            break;
                        case "*":
                            Display = display + "*";
                            break;
                        case "-":
                            Display = display + "-";
                            break;
                        case "+":
                            Display = display + "+";
                            break;
                        case "^":
                            Display = display + "^";
                            break;
                        case "Sqrt":
                            if (display == "" || (!Char.IsDigit(Display.Last()) && Display.Last() != ')'))
                            {
                                Display = display + "Sqrt(";
                                leftBreackets += 1;
                            }
                            break;
                        case "%":
                            if (leftBreackets == 0) GetSolution(100);
                            break;
                        case "=":
                            if (leftBreackets == 0) GetSolution();
                            break;
                    }

                    if (!operation.Equals(")")
                        && !operation.Equals("=")
                        && !operation.Equals("(")
                        && !operation.Equals("Sqrt")
                        && !operation.Equals("%")) LastOperation = operation;
                }

            }
            catch (Exception ex)
            {
                Display = Result == string.Empty ? "Error" : Result;
            }
        }

        private void GetSolution(int? percent = null)
        {
            InputExpression = display;

            if (percent == null)
            {
                GetResult();
            }
            else
            {
                for (int i = InputExpression.Length; i >= 0; i--)
                {
                    char c = InputExpression[i - 1];

                    if (Char.IsDigit(c)) { }
                    else if (c == '*')
                    {
                        InputExpression = "(" + display.Insert(--i, ")") + "/" + percent.ToString();

                        //InputExpression = "(" + display + ")" + "/" + percent.ToString();
                        GetResult();

                        break;
                    }
                    else
                        break;
                }
            }
        }

        public void GetResult()
        {
            calculation.CalculateResult();
            Display = Result;
            FullExpression = PostfixExpr;

            LastOperation = string.Empty;
            leftBreackets = 0;
            newDisplayRequired = true;
        }

        #endregion
    }
}
