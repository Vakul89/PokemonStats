using Backend.Models;
using System.Text.Json;

namespace UnitTest.Models
{
    public class PokemonDTOTests
    {
        [Fact]
        public void PokemonDTO_DefaultsAndSetters()
        {
            var dto = new PokemonDTO();

            // defaults
            Assert.Equal(0, dto.Id);
            Assert.Equal(string.Empty, dto.Name);
            Assert.Equal(string.Empty, dto.Type);
            Assert.Equal(string.Empty, dto.ImageUrl);

            // set values
            dto.Id = 25;
            dto.Name = "Pikachu";
            dto.Type = "Electric";
            dto.Wins = 5;
            dto.Losses = 2;
            dto.Ties = 1;
            dto.Base_Experience = 112;
            dto.ImageUrl = "http://example.com/pikachu.png";

            Assert.Equal(25, dto.Id);
            Assert.Equal("Pikachu", dto.Name);
            Assert.Equal("Electric", dto.Type);
            Assert.Equal(5, dto.Wins);
            Assert.Equal(2, dto.Losses);
            Assert.Equal(1, dto.Ties);
            Assert.Equal(112, dto.Base_Experience);
            Assert.Equal("http://example.com/pikachu.png", dto.ImageUrl);
        }

        [Fact]
        public void PokemonDTO_SerializesAndDeserializes()
        {
            var dto = new PokemonDTO
            {
                Id = 150,
                Name = "Mewtwo",
                Type = "Psychic",
                Wins = 99,
                Losses = 0,
                Ties = 0,
                Base_Experience = 306,
                ImageUrl = "http://example.com/mewtwo.png"
            };

            var json = JsonSerializer.Serialize(dto);
            var dto2 = JsonSerializer.Deserialize<PokemonDTO>(json);

            Assert.NotNull(dto2);
            Assert.Equal(dto.Id, dto2!.Id);
            Assert.Equal(dto.Name, dto2.Name);
            Assert.Equal(dto.Type, dto2.Type);
            Assert.Equal(dto.Wins, dto2.Wins);
            Assert.Equal(dto.Losses, dto2.Losses);
            Assert.Equal(dto.Ties, dto2.Ties);
            Assert.Equal(dto.Base_Experience, dto2.Base_Experience);
            Assert.Equal(dto.ImageUrl, dto2.ImageUrl);
        }
    }
}
