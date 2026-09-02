using API.Models;

namespace API.Interfaces
{
    public interface IPokemonService
    {
        Task<List<PokemonDTO>> GetRandomPokemonAsync(int pokemonFetchCount);
    }
}
