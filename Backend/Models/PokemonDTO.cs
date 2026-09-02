namespace API.Models
{
    public class PokemonDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Ties { get; set; }

        public int Base_Experience { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
    }
}
