using System;
using Core.Interface;
using System.Configuration;
using MySql.Data.MySqlClient;


namespace Core
{
    public class Connect : IConnect
    {
        private static Connect _connect;
        public MySqlConnection GetConnect { get; private set; }
        private string connectionString = string.Empty;

        private Connect() {}

        public static Connect Instance()
        {
            if (_connect == null)
                _connect = new Connect();
            return _connect;
        }

        private void ReadConfig()
        {
            try
            {
                var str = ConfigurationManager.ConnectionStrings["MySQLConnect"];
                _connect.connectionString = str.ConnectionString;
            }
            catch (Exception ex)
            {
                _connect.ConnectionErrorEvent(ex.Message, "Ошибка подключения");
            }
        }

        public bool IsConnect()
        {
            if (connectionString == string.Empty)
                ReadConfig();
            if (connectionString.Length == 0)
            {
                ConnectionErrorEvent("Конфигурация подключения пуста", "Ошибка подключения");
                return false;
            }
            try
            {
                GetConnect = new MySqlConnection(connectionString);
                GetConnect.Open();
            }
            catch(Exception ex)
            {
                ConnectionErrorEvent(ex.Message, "Ошибка подключения");
                return false;
            }

            return true;
        }

        public void Close()
        {
            GetConnect.Close();
            GetConnect.Dispose();
        }
        public event Action<string, string> ConnectionErrorEvent;
    }
}
