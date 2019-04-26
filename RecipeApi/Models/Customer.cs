using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace RecipeApi.Models
{
    public class Customer
    {
        #region Properties
        //add extra properties if needed
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<CustomerRating> Ratings { get; private set; }

        public IEnumerable<KeyValuePair<Recipe, int>> RatedRecipes
         {
            get
            {
                var dict = new Dictionary<Recipe, int>();
                foreach (var x in Ratings)
                {
                    dict.TryAdd(x.Recipe, x.Rating);
                }
                return dict;
            }
        }
        #endregion

        #region Constructors
        public Customer()
        {
            Ratings = new List<CustomerRating>();
        }
        #endregion

        #region Methods
       
        public void RateRecipe(Recipe recipe, int rating)
        {
            foreach (var el in Ratings.Where(item => item.Recipe == recipe && item.CustomerId == CustomerId).ToList())
            {
                Ratings.Remove(el);
            }
            Ratings.Add(new CustomerRating() { RecipeId = recipe.Id, CustomerId = CustomerId, Recipe = recipe, Customer = this, Rating = rating });
        }
        #endregion
    }
}
