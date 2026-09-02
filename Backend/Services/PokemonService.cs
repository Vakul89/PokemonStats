using API.Interfaces;
using API.Models;
using System.Text.Json.Serialization;

namespace API.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly HttpClient _httpClient;
        private readonly Random _random = new();

        public PokemonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
        }

        public async Task<List<PokemonDTO>> GetRandomPokemonAsync(int pokemonFetchCount)
        {
            var result = new List<PokemonDTO>();
            var UsedIds = new HashSet<int>();

            while (result.Count < pokemonFetchCount)
            {
                int id = _random.Next(1, 152);
                if (!UsedIds.Add(id))
                    continue;

                var response = await _httpClient.GetFromJsonAsync<PokemonApiResponse>($"pokemon/{id}");
                if (response == null)
                    continue;

                var primaryType = response.Types.OrderBy(t => t.Slot).First().Type.Name;

                result.Add(new PokemonDTO
                {
                    Id = response.Id,
                    Name = response.Name,
                    Type = primaryType,
                    Base_Experience = response.Base_Experience,
                    ImageUrl = response.Sprites.Front_Default
                });
            }

            return result;
        }
    }
}
