using System.Collections.Generic;

namespace RecipeApi.Models
{
    public class CustomerRating
    {
        #region Properties
        public int CustomerId { get; set; }

        public int RecipeId { get; set; }

        public Customer Customer { get; set; }

        public Recipe Recipe { get; set; }

        public int Rating { get; set; }
        #endregion
    }
}
