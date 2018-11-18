using Assistant.ViewModel;

namespace Assistant.Interface
{
    interface INotifyContentChanged
    {
        void ChangeContent(BaseViewModel sender, BaseViewModel newContent);
    }
}
