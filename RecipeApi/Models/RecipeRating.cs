using System;
using Microsoft.AspNetCore.Identity;

namespace RecipeApi.Models
{
    public class RecipeRating
    {
        #region Properties
        public int UserId { get; set; }

        public int RecipeId { get; set; }

        public IdentityUser User { get; set; }

        public Recipe Recipe { get; set; }

        public int Rating { get; set; }
        #endregion
    }
}
