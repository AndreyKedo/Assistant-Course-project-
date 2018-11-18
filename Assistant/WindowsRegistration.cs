using Core;
using Core.Interface;
using System.Collections.Generic;
using System.Windows.Controls;
using Assistant.Interface;
using Assistant.View;

namespace Assistant
{
    class WindowsRegistration
    {
        private Dictionary<Group, IViewModel> ContextData;
        public IWindow CurrentWindow { get; private set; }

        public WindowsRegistration()
        {
            CurrentWindow = new MainWindow();
            ContextData = new Dictionary<Group, IViewModel>();
        }

        public void RegistrationContext(Group group, IViewModel vm, UserControl control)
        {
            vm.CurrentContent = control;
            ContextData.Add(group, vm);
        }

        public void SpawnWindow(Group group, IEmployees employees)
        {
            IViewModel vm;
            if (group.Equals(Group.Registrator))
            {
                CurrentWindow.Title = "Assistant: Регистратура";
                if(ContextData.TryGetValue(group, out vm))
                {
                    vm.Employees = employees;
                    CurrentWindow.DataContext = vm;
                }
            }else if (group.Equals(Group.Doctor))
            {
                CurrentWindow.Title = "Assistant: Специалист";
                if (ContextData.TryGetValue(group, out vm))
                {
                    vm.Employees = employees;
                    CurrentWindow.DataContext = vm;
                }
            }
            if (CurrentWindow.DataContext != null)
                CurrentWindow.Show();
        }
        public void Close()
        {
            CurrentWindow.Close();
        }
    }
}
