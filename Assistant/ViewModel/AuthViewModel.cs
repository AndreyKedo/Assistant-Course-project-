using System;
using System.Windows.Controls;
using System.Windows.Input;
using Core;
using Core.Interface;

namespace Assistant.ViewModel
{
    class AuthViewModel : MainViewModel
    {
        IAuthorization _authorization;
        IConnect _connect;
        public AuthViewModel(IConnect connect)
        {
            _connect = connect;
            _authorization = Authotization.Instance(connect);
            _authorization.AuthoStatysEvent += StatysEvent;
        }

        private string _login;
        public string Login
        {
            get => _login; 
            set
            {
                _login = value;
                OnChanged(nameof(Login));
            }
        }

        private string _statys;
        public string Statys
        {
            get => _statys;
            set
            {
                _statys = value;
                OnChanged(nameof(Statys));
            }
        }

        public ICommand Auth
        {
            get
            {
                return new DelegateCommand(async (obj) =>
                {
                    await _authorization.Auth(Login, (obj as PasswordBox).Password);
                    Login = string.Empty;
                    (obj as PasswordBox).Clear();
                    if (_authorization.GetEmployees != null)
                    {
                        ConnectionEvent(_authorization.GetEmployees, _authorization.GetGroup);
                    }
                }, (obj) =>
                 {
                     return Login != string.Empty && (obj as PasswordBox).Password != string.Empty;
                 });
            }
        }

        private void StatysEvent(string str)
        {
            Statys = str;
        }

        public event Action<IEmployees, Group> ConnectionEvent;
    }
}
