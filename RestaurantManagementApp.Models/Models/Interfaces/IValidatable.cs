using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.Models.Interfaces
{
    public interface IValidatable
    {
        bool IsValid();
        IEnumerable<string> GetValidationErrors();
    }
}
