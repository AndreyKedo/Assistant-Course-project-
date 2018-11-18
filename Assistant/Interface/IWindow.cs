namespace Assistant.Interface
{
    interface IWindow
    {
        string Title { get; set; }
        void Show();
        void Close();
        object DataContext { get; set; }
    }
}
