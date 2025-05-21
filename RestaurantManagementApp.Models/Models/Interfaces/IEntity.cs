using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagementApp.Models.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public abstract class EntityBase : IEntity
    {
       
        public int Id { get; set; }
    }
}