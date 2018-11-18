using System.Windows;

namespace Assistant
{
    public partial class App : Application
    {
        private Init _init;
        protected override void OnStartup(StartupEventArgs e)
        {
            _init = Init.Instance();
            _init.Run(this);
        }
    }
}
