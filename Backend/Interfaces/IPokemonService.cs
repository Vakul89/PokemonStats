using Backend.Models;

namespace Backend.Interfaces
{
    public interface IPokemonService
    {
        Task<List<PokemonDTO>> GetRandomPokemonAsync(int pokemonFetchCount);
    }
}
