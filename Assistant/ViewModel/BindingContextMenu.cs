using System.Windows;

namespace Assistant.ViewModel
{
    /// <summary>
    /// Класс является прослойко между VM и context menu
    /// Тут регистрируется DependencyProperty и создаётся свойство для передачи контекста данных
    /// Класс наследуеться от Freezable
    /// </summary>
    class BindingContextMenu : Freezable
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Proxy", typeof(object), typeof(BindingContextMenu), new UIPropertyMetadata(null));

        public object Proxy
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingContextMenu();
        }
    }
}
