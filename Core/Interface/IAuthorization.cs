using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IAuthorization
    {
        Group GetGroup { get; }
        IEmployees GetEmployees { get; }
        Task Auth(string login, string pwd);
        event System.Action<string> AuthoStatysEvent;
    }
}
