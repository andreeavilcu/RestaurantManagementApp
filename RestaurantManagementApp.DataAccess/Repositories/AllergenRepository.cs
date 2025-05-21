using RestaurantManagementApp.DataAccess.Repositories.Implementations;
using RestaurantManagementApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace RestaurantManagementApp.DataAccess.Repositories
{
    public interface IAllergenRepository : IRepository<Models.Allergen>
    {
        IEnumerable<Models.Allergen> GetAllergensWithProducts();
    }

    public class AllergenRepository : Repository<Allergen>, IAllergenRepository
    {
        public AllergenRepository(DatabaseContext context) : base(context)
        {
        }

        public IEnumerable<Allergen> GetAllergensWithProducts()
        {
            return _context.Allergens
                .Include(a => a.ProductAllergens.Select(pa => pa.Product))
                .ToList();
        }
    }
}
