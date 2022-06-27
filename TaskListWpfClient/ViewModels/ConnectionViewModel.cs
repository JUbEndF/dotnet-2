using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskListWpfClient.ViewModels
{
    public class ConnectionViewModel
    {
        public string ConnectionAddress { get; set; } = string.Empty;
        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ConnectionViewModel()
        {
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void Add()
        {
            if (ConnectionAddress == String.Empty)
            {
                MessageBox.Show("The name string cannot be empty");
                return;
            }

            try{
                MainWindow mainWindow = new(ConnectionAddress);
                mainWindow.Show();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(ConnectionWindow))
                    {
                        window.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Cancel()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(ConnectionWindow))
                {
                    window.Close();
                }
            }
        }
    }
}
