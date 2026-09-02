using API.Helpers;
using API.Interfaces;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PokemonController(IPokemonService pokemonService, PokemonBattleEngine battleEngine) : Controller
    {
        private readonly IPokemonService _pokemonService = pokemonService;
        private readonly PokemonBattleEngine _battleEngine = battleEngine;

        private static readonly string[] ValidaSortBy = ["id", "name", "wins", "losses", "ties"];

        private static readonly string[] ValidSortDirection = ["asc", "desc"];

        [HttpGet("pokemon/tournament/statistics")]
        public async Task<IActionResult> GetPokemons([FromQuery] string sortBy, [FromQuery] string sortDirection)
        {
            if (string.IsNullOrEmpty(sortBy))
                return BadRequest("sortBy parameter is required.");

            else if (string.IsNullOrEmpty(sortDirection))
                return BadRequest("sortDirection parameter is required.");

            else if (!ValidaSortBy.Contains(sortBy.ToLower()))
                return BadRequest("sortBy parameter is invalid.");

            else if (!ValidSortDirection.Contains(sortDirection.ToLower()))
                return BadRequest("sortDirection parameter is invalid.");

            var pokemons = await _pokemonService.GetRandomPokemonAsync(16); // Fetch 16 random Pokémon
            _battleEngine.RunRoundRobin(pokemons);

            pokemons = SortPokemons(pokemons, sortBy!, sortDirection!);

            var response = pokemons.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                type = p.Type,
                wins = p.Wins,
                losses = p.Losses,
                ties = p.Ties,
                imageUrl = p.ImageUrl
            });

            return Ok(pokemons);
        }

        private static List<PokemonDTO> SortPokemons(
        List<PokemonDTO> pokemons, string sortBy, string sortDirection)
        {
            Func<PokemonDTO, object> keySelector = sortBy.ToLower() switch
            {
                "wins" => p => p.Wins,
                "losses" => p => p.Losses,
                "ties" => p => p.Ties,
                "name" => p => p.Name,
                "id" => p => p.Id,
                _ => p => p.Id
            };

            return sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? pokemons.OrderByDescending(keySelector).ToList()
                : pokemons.OrderBy(keySelector).ToList();
        }
    }
}