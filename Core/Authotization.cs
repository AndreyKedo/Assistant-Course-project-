using System;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Core.Interface;
using Core.Model;
using System.Security.Cryptography;

namespace Core
{
    public enum Group
    {
        Registrator = 1122,
        Doctor = 2211
    }

    public class Authotization : IAuthorization
    {
        private static Authotization _authotization;
        private IConnect connect;
        private uint idGroup = 0;
        public Group GetGroup { get; private set; }
        public byte[] HashID { get; private set; } // массив битов
        public IEmployees GetEmployees { get; private set; }

        public static Authotization Instance(IConnect connect)
        {
            if (_authotization == null)
                _authotization = new Authotization();
            _authotization.connect = connect;
            return _authotization;
        }

        public async Task Auth(string login, string pwd)
        {
            if (connect.IsConnect())
            {
                string authQuery = "SELECT id_user, id_group FROM accounts " +
                    "WHERE login = @login AND pwd = @pwd LIMIT 1;";
                using (MySqlCommand command = new MySqlCommand(authQuery, connect.GetConnect))
                {
                    command.Parameters.AddWithValue("@login", login); //логин
                    command.Parameters.AddWithValue("@pwd", GetHash(MD5.Create(), pwd)); //пароль
                    using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                HashID = Encoding.UTF8.GetBytes(reader.GetString(0)); //перевод хэша в массив битов
                                idGroup = await reader.GetFieldValueAsync<uint>(1);
                            }
                            reader.Close();
                        }
                        else
                        {
                            AuthoStatysEvent("Неправильный логин или пароль");
                        }
                    }
                }
                if (idGroup.Equals(1122)) //Регистратор
                {
                    string selectRegistratorQuery = "SELECT id, fio FROM registrator " +
                        "WHERE id_user = @idUser";
                    using (MySqlCommand command = new MySqlCommand(selectRegistratorQuery, connect.GetConnect))
                    {
                        string id = Encoding.UTF8.GetString(HashID); //перевод массива битов в хэш
                        command.Parameters.AddWithValue("@idUser", id);
                        using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    GetEmployees = new Registrator()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(0),
                                        Fio = reader.GetString(1)
                                    };
                                }
                                reader.Close();
                            }
                        }
                    }
                    GetGroup = Group.Registrator;
                }else if (idGroup.Equals(2211)) //Доктор
                {
                    string selectRegistratorQuery = "SELECT id, fio FROM doctor WHERE id_user = @idUser";
                    using (MySqlCommand command = new MySqlCommand(selectRegistratorQuery, connect.GetConnect))
                    {
                        string id = Encoding.UTF8.GetString(HashID); //перевод массива битов в хэш
                        command.Parameters.AddWithValue("@idUser", id);
                        using (System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    GetEmployees = new Doctor()
                                    {
                                        Id = await reader.GetFieldValueAsync<uint>(0),
                                        Fio = reader.GetString(1)
                                    };
                                }
                                reader.Close();
                            }
                        }
                    }
                    GetGroup = Group.Doctor;
                }
                connect.Close();
            }
        }

        private string GetHash(HashAlgorithm algorithm, string str)
        {
            var hashPwd = algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder pwdHash = new StringBuilder();
            for (int i = 0; i < hashPwd.Length; i++)
            {
                pwdHash.Append(hashPwd[i].ToString("x2"));
            }
            return pwdHash.ToString();
        }

        public event Action<string> AuthoStatysEvent;
    }
}
