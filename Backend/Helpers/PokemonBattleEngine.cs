using API.Models;

namespace API.Helpers
{
    public class PokemonBattleEngine
    {
        private readonly Dictionary<string, string> FigtingTypeAdvantages = new()
        {
            ["water"] = "fire",
            ["fire"] = "grass",
            ["grass"] = "electric",
            ["electric"] = "water",
            ["ghost"] = "psychic",
            ["psychic"] = "fighting",
            ["fighting"] = "dark",
            ["dark"] = "ghost"
        };

        public void RunRoundRobin(List<PokemonDTO> pokemons)
        {
            for (int i = 0; i < pokemons.Count; i++)
            {
                for (int j = i + 1; j < pokemons.Count; j++)
                {
                    ResolveBattle(pokemons[i], pokemons[j]);
                }
            }
        }

        private void ResolveBattle(PokemonDTO p1, PokemonDTO p2)
        {
            if (FigtingTypeAdvantages.TryGetValue(p1.Type, out var advantage) && advantage == p2.Type)
            {
                p1.Wins++;
                p2.Losses++;
                return;
            }
            if (FigtingTypeAdvantages.TryGetValue(p2.Type, out var advantage2) && advantage2 == p1.Type)
            {
                p2.Wins++;
                p1.Losses++;
                return;
            }

            if (p1.Base_Experience > p2.Base_Experience)
            {
                p1.Wins++;
                p2.Losses++;
            }
            else if (p2.Base_Experience > p1.Base_Experience)
            {
                p2.Wins++;
                p1.Losses++;
            }
            else
            {
                p2.Ties++;
                p1.Ties++;
            }
        }
    }
}
