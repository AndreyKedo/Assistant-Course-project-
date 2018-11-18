using System.Windows.Controls;
using Core.Interface;

namespace Assistant.Interface
{
    interface IViewModel
    {
        UserControl CurrentContent { get; set; }
        IEmployees Employees { get; set; }
    }
}
