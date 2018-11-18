using System.ComponentModel;

namespace Assistant.Interface
{
    interface IMainViewModel : INotifyPropertyChanged
    {
        void OnChanged(string str);
    }
}
