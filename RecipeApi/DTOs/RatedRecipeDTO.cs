using System;
using System.ComponentModel.DataAnnotations;
using RecipeApi.Models;

namespace RecipeApi.DTOs
{
    public class RatedRecipeDTO
    {
        [Required]
        public int Id { get; set; }


        [Required]
        public int Rating { get; set; }

        public RatedRecipeDTO(Recipe recipe, int rating)
        {
            Id = recipe.Id;
            Rating = rating;
        }
    }
}
