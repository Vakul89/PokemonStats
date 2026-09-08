using Backend.Models;
using System.Text.Json;

namespace UnitTest.Models
{
    public class PokemonApiResponseTests
    {
        [Fact]
        public void PokemonApiResponse_DefaultsAndSetters()
        {
            var resp = new PokemonApiResponse();

            // defaults
            Assert.Equal(0, resp.Id);
            Assert.Equal(string.Empty, resp.Name);
            Assert.Equal(0, resp.Base_Experience);
            Assert.NotNull(resp.Types);
            Assert.Empty(resp.Types);
            Assert.NotNull(resp.Sprites);
            Assert.Equal(string.Empty, resp.Sprites.Front_Default);

            // set values
            resp.Id = 1;
            resp.Name = "Bulbasaur";
            resp.Base_Experience = 64;
            resp.Types.Add(new SlotType { Slot = 1, Type = new Backend.Models.Type { Name = "grass" } });
            resp.Sprites.Front_Default = "http://example.com/bulbasaur.png";

            Assert.Equal(1, resp.Id);
            Assert.Equal("Bulbasaur", resp.Name);
            Assert.Equal(64, resp.Base_Experience);
            Assert.Single(resp.Types);
            Assert.Equal(1, resp.Types[0].Slot);
            Assert.Equal("grass", resp.Types[0].Type.Name);
            Assert.Equal("http://example.com/bulbasaur.png", resp.Sprites.Front_Default);
        }

        [Fact]
        public void PokemonApiResponse_SerializesAndDeserializes()
        {
            var resp = new PokemonApiResponse
            {
                Id = 150,
                Name = "Mewtwo",
                Base_Experience = 306,
                Types = new List<SlotType>
                {
                    new SlotType { Slot = 1, Type = new Backend.Models.Type { Name = "psychic" } }
                },
                Sprites = new Sprites { Front_Default = "http://example.com/mewtwo.png" }
            };

            var json = JsonSerializer.Serialize(resp);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resp2 = JsonSerializer.Deserialize<PokemonApiResponse>(json, options);

            Assert.NotNull(resp2);
            Assert.Equal(resp.Id, resp2!.Id);
            Assert.Equal(resp.Name, resp2.Name);
            Assert.Equal(resp.Base_Experience, resp2.Base_Experience);
            Assert.Single(resp2.Types);
            Assert.Equal("psychic", resp2.Types[0].Type.Name);
            Assert.Equal(resp.Sprites.Front_Default, resp2.Sprites.Front_Default);
        }

        [Fact]
        public void PokemonApiResponse_Deserialize_FromApiStyleJson()
        {
            string json = @"{
                                ""id"": 25, ""name"": ""Pikachu"", ""base_experience"": 112,
                                ""types"": [ { ""slot"": 1, ""type"": { ""name"": ""electric"" } } ],
                                ""sprites"": { ""front_default"": ""http://example.com/pikachu.png"" } 
                            }";

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resp = JsonSerializer.Deserialize<PokemonApiResponse>(json, options);

            Assert.NotNull(resp);
            Assert.Equal(25, resp!.Id);
            Assert.Equal("Pikachu", resp.Name);
            Assert.Equal(112, resp.Base_Experience);
            Assert.Single(resp.Types);
            Assert.Equal("electric", resp.Types[0].Type.Name);
            Assert.Equal("http://example.com/pikachu.png", resp.Sprites.Front_Default);
        }
    }
}
