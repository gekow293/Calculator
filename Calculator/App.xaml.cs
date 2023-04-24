using Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Calculator.Views;

namespace Calculator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow view = new MainWindow();
            view.DataContext = new CalculatorViewModel();
            view.Show();
        }
    }
}
