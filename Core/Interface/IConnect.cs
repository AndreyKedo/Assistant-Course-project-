using System;
using MySql.Data.MySqlClient;

namespace Core.Interface
{
    public interface IConnect
    {
        MySqlConnection GetConnect { get; }
        bool IsConnect();
        void Close();
        event Action<string, string> ConnectionErrorEvent;
    }
}
