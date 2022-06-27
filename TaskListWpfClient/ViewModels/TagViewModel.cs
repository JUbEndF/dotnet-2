using ReactiveUI;
using System;
using System.Reactive;
using System.Windows;
using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.ViewModels
{
    public class TagViewModel
    {
        public string TagName { get; set; } = string.Empty;
        public TagProto Tag { get; set; } = new();
        private readonly MainViewModel _mainViewModel;
        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public TagViewModel(MainViewModel mainViewModel, TagProto tagProto)
        {
            _mainViewModel = mainViewModel;
            Tag = tagProto;
            TagName = tagProto.Name;
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        public TagViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void Add()
        {
            if (TagName == String.Empty)
            {
                MessageBox.Show("The name string cannot be empty");
                return;
            }
            if (Tag.Id == 0)
                _mainViewModel.AddTag(new TagProto() { Id = Tag.Id, Name = TagName });
            else _mainViewModel.UpdateTag(new TagProto() { Id = Tag.Id, Name = TagName });
            _mainViewModel.UpdateDatabase();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    window.Show();
                }
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(TagWindow))
                {
                    window.Close();
                }
            }
        }

        private void Cancel()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    window.Show();
                }
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(TagWindow))
                {
                    window.Close();
                }
            }
        }
    }
}
