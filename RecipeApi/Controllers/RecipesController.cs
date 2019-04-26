using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApi.DTOs;
using RecipeApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecipeApi.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICustomerRepository _customerRepository;

        public RecipesController(IRecipeRepository recipeRepository, ICustomerRepository customerRepository)
        {
            _recipeRepository = recipeRepository;
            _customerRepository = customerRepository;
        }

        // GET: api/Recipes
        /// <summary>
        /// Get all recipes ordered by name
        /// </summary>
        /// <returns>array of recipes</returns>
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<Recipe> GetRecipes(string name = null, string chef = null, string ingredientName = null)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(chef) && string.IsNullOrEmpty(ingredientName))
                return _recipeRepository.GetAll();
            return _recipeRepository.GetBy(name, chef, ingredientName);
        }

        // GET: api/Recipes/5
        /// <summary>
        /// Get the recipe with given id
        /// </summary>
        /// <param name="id">the id of the recipe</param>
        /// <returns>The recipe</returns>
        [HttpGet("{id}")]
        public ActionResult<Recipe> GetRecipe(int id)
        {
            Recipe recipe = _recipeRepository.GetBy(id);
            if (recipe == null) return NotFound();
            return recipe;
        }

        /// <summary>
        /// Get ratings of recipes with the provided id's by current user
        /// </summary>
        [HttpGet("Rated")]
        public IEnumerable<RatedRecipeDTO> GetRatedRecipes([FromQuery(Name = "id")] int[] recipeIds)
        {
            Customer customer = _customerRepository.GetBy(User.Identity.Name);
            List<RatedRecipeDTO> ratedRecipes = new List<RatedRecipeDTO>();
            foreach (var ratedRecipe in customer.RatedRecipes.Where(p => recipeIds.Contains(p.Key.Id)))
            {
                ratedRecipes.Add(new RatedRecipeDTO(ratedRecipe.Key, ratedRecipe.Value));
            }
            return ratedRecipes;
        }

        /// <summary>
        /// Rate a recipe with a certain id
        /// </summary>
        /// <param name="id">id of the recipe being rated</param>
        /// <param name="rating">rating of the recipe</param>
        [HttpPut("Rate/{id}/{rating}")]
        public ActionResult<RatedRecipeDTO> RateRecipe(int id, int rating)
        {
            Customer customer = _customerRepository.GetBy(User.Identity.Name);
            Recipe recipe = _recipeRepository.GetBy(id);
            if (recipe == null)
            {
                return NotFound();
            }
            customer.RateRecipe(recipe, rating);
            _customerRepository.SaveChanges();
            return new RatedRecipeDTO(recipe, rating);
        }

        // POST: api/Recipes
        /// <summary>
        /// Adds a new recipe
        /// </summary>
        /// <param name="recipe">the new recipe</param>
        [HttpPost]
        public ActionResult<Recipe> PostRecipe(RecipeDTO recipe)
        {
            Recipe recipeToCreate = new Recipe() { Name = recipe.Name, Chef = recipe.Chef };
            foreach (var i in recipe.Ingredients)
                recipeToCreate.AddIngredient(new Ingredient(i.Name, i.Amount, i.Unit));
            _recipeRepository.Add(recipeToCreate);
            _recipeRepository.SaveChanges();

            return CreatedAtAction(nameof(GetRecipe), new { id = recipeToCreate.Id }, recipeToCreate);
        }

        // PUT: api/Recipes/5
        /// <summary>
        /// Modifies a recipe
        /// </summary>
        /// <param name="id">id of the recipe to be modified</param>
        /// <param name="recipe">the modified recipe</param>
        [HttpPut("{id}")]
        public IActionResult PutRecipe(int id, Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest();
            }
            _recipeRepository.Update(recipe);
            _recipeRepository.SaveChanges();
            return NoContent();
        }

        // DELETE: api/Recipes/5
        /// <summary>
        /// Deletes a recipe
        /// </summary>
        /// <param name="id">the id of the recipe to be deleted</param>
        [HttpDelete("{id}")]
        public ActionResult<Recipe> DeleteRecipe(int id)
        {
            Recipe recipe = _recipeRepository.GetBy(id);
            if (recipe == null)
            {
                return NotFound();
            }
            _recipeRepository.Delete(recipe);
            _recipeRepository.SaveChanges();
            return recipe;
        }

        /// <summary>
        /// Get an ingredient for a recipe
        /// </summary>
        /// <param name="id">id of the recipe</param>
        /// <param name="ingredientId">id of the ingredient</param>
        [HttpGet("{id}/ingredients/{ingredientId}")]
        public ActionResult<Ingredient> GetIngredient(int id, int ingredientId)
        {
            if (!_recipeRepository.TryGetRecipe(id, out var recipe))
            {
                return NotFound();
            }
            Ingredient ingredient = recipe.GetIngredient(ingredientId);
            if (ingredient == null)
                return NotFound();
            return ingredient;
        }

        /// <summary>
        /// Adds an ingredient to a recipe
        /// </summary>
        /// <param name="id">the id of the recipe</param>
        /// <param name="ingredient">the ingredient to be added</param>
        [HttpPost("{id}/ingredients")]
        public ActionResult<Ingredient> PostIngredient(int id, IngredientDTO ingredient)
        {
            if (!_recipeRepository.TryGetRecipe(id, out var recipe))
            {
                return NotFound();
            }
            var ingredientToCreate = new Ingredient(ingredient.Name, ingredient.Amount, ingredient.Unit);
            recipe.AddIngredient(ingredientToCreate);
            _recipeRepository.SaveChanges();
            return CreatedAtAction("GetIngredient", new { id = recipe.Id, ingredientId = ingredientToCreate.Id }, ingredientToCreate);
        }
    }
}