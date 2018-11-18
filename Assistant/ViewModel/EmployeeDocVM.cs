using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using Assistant.Interface;

namespace Assistant.ViewModel
{
    class EmployeeDocVM : BaseViewModel, IViewModel
    {
        public EmployeeDocVM(IDataAccessDoctor dataAccess){
        }
    }
}
