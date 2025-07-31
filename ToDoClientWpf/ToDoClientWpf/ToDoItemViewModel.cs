using System.ComponentModel;

namespace ToDoClientWpf
{
    public class ToDoItemViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;

        private bool _completed;
        public bool Completed
        {
            get => _completed;
            set
            {
                if (_completed != value)
                {
                    _completed = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Completed)));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
