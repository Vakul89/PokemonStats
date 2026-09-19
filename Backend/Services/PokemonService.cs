using Backend.Interfaces;
using Backend.Models;

namespace Backend.Services
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
            var usedIds = new HashSet<int>();

            const int maxId = 151; // PokeAPI first generation
            const int maxConcurrency = 10; // throttle concurrent HTTP requests for scale
            using var semaphore = new SemaphoreSlim(maxConcurrency);

            // Continue until we have the requested number of unique Pokemon
            while (result.Count < pokemonFetchCount)
            {
                var remaining = pokemonFetchCount - result.Count;

                // Pick a batch of unique ids to fetch in parallel
                var idsToFetch = new List<int>(remaining);
                while (idsToFetch.Count < remaining)
                {
                    var id = _random.Next(1, maxId + 1);
                    if (usedIds.Add(id))
                        idsToFetch.Add(id);
                }

                // Fire off parallel requests with limited concurrency
                var fetchTasks = idsToFetch.Select(async id =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var response = await _httpClient.GetFromJsonAsync<PokemonApiResponse>($"pokemon/{id}");
                        if (response == null)
                            return null;

                        var primaryType = response.Types.OrderBy(t => t.Slot).First().Type.Name;

                        return new PokemonDTO
                        {
                            Id = response.Id,
                            Name = response.Name,
                            Type = primaryType,
                            Base_Experience = response.Base_Experience,
                            ImageUrl = response.Sprites.Front_Default
                        };
                    }
                    catch
                    {
                        // swallow and return null for failed fetches; we'll try new ids in the next iteration
                        return null;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }).ToList();

                var fetched = await Task.WhenAll(fetchTasks);
                foreach (var p in fetched)
                {
                    if (p != null)
                        result.Add(p);
                }

                // If some fetches failed we loop and request additional ids until we reach the desired count
            }

            return result.Take(pokemonFetchCount).ToList();
        }
    }
}
