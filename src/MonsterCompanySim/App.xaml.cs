using MonsterCompanySim.ViewModels;
using MonsterCompanySim.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MonsterCompanySim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var w = new MainView();
            var vm = new MainViewModel();

            w.DataContext = vm;
            w.Show();
        }
    }
}
