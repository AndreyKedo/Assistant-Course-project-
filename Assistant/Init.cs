using System;
using Core;
using Core.Interface;
using System.Windows;
using Assistant.View;
using Assistant.ViewModel;

namespace Assistant
{
    class Init
    {
        private static Init _initApp = null;
        IConnect ConnectApp { get; set; }
        private WindowsRegistration registration;
        private Application app;
        private AuthorizationWindow AuthWindow;
        private AuthViewModel AuthVM;

        private Init()
        {
            AuthWindow = new AuthorizationWindow();
            ConnectApp = Connect.Instance();
        }

        public static Init Instance()
        {
            if (_initApp == null)
                _initApp = new Init();
            _initApp.registration = new WindowsRegistration(); //регистрация главного окна
            _initApp.ConnectApp.ConnectionErrorEvent += _initApp.ErrorMessage;
            _initApp.AuthVM = new AuthViewModel(_initApp.ConnectApp);
            _initApp.AuthVM.ConnectionEvent += _initApp.CreateMainWindow;
            //регистрация контекста окна
            _initApp.registration.RegistrationContext(Group.Registrator,new EmployeeRegVM(new DataAccess(_initApp.ConnectApp)), new RegistratorUC());
            _initApp.registration.RegistrationContext(Group.Doctor, new EmployeeDocVM(new DataAccess(_initApp.ConnectApp)), new DoctorUC());
            return _initApp;
        }

        public void Run(Application app)
        {
            this.app = app;
            ShowAuthWindow();
        }

        private void CreateMainWindow(IEmployees employees, Group group)
        {
            try
            {
                registration.SpawnWindow(group, employees);
                AuthWindow.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage(ex.Message, "Assistant");
                app.Shutdown(1);
            }
        }

        private void ShowAuthWindow()
        {
            if (ConnectApp.IsConnect())
            {
                if(AuthWindow.DataContext == null)
                    AuthWindow.DataContext = AuthVM;
                AuthWindow.Show();
                ConnectApp.Close();
            }
            else
            {
                app.Shutdown(1);
            }
        }

        private void ErrorMessage(string str, string caption)
        {
            MessageBox.Show(str, "Ошибка: " + caption);
        }
    }
}
