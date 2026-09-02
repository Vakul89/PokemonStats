using System.Text.Json.Serialization;

namespace API.Models
{
    public class PokemonApiResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Base_Experience { get; set; }

        public List<SlotType> Types { get; set; } = new();

        public Sprites Sprites { get; set; } = new();
    }

    public class SlotType
    {
        public int Slot { get; set; }
        public Type Type { get; set; } = new();
    }

    public class Type
    {
        public string Name { get; set; } = string.Empty;
    }

    public class Sprites
    {
        public string Front_Default { get; set; } = string.Empty;
    }

}
