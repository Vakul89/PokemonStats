using Backend.Models;
using Backend.Services;
using System.Net;
using System.Text;
using System.Text.Json;

namespace UnitTest.Services
{
    class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Extract id from the request path
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            // Expect path like /api/v2/pokemon/25 or /pokemon/25 depending on BaseAddress
            var parts = path.Trim('/').Split('/');
            int id = 1;
            if (parts.Length > 0 && int.TryParse(parts[^1], out var parsed))
                id = parsed;

            var resp = new PokemonApiResponse
            {
                Id = id,
                Name = $"Pokemon{id}",
                Base_Experience = id * 10,
                Types = new List<SlotType>
                {
                    new SlotType { Slot = 2, Type = new Backend.Models.Type { Name = "secondary" } },
                    new SlotType { Slot = 1, Type = new Backend.Models.Type { Name = "primary" } }
                },
                Sprites = new Sprites { Front_Default = $"http://example.com/{id}.png" }
            };

            var json = JsonSerializer.Serialize(resp);
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(message);
        }
    }

    public class PokemonServiceTests
    {
        private static HttpClient CreateClient()
        {
            var handler = new FakeHttpMessageHandler();
            return new HttpClient(handler);
        }

        [Fact]
        public async Task GetRandomPokemonAsync_ReturnsRequestedCount_And_MapsFields()
        {
            var client = CreateClient();
            var service = new PokemonService(client);

            var list = await service.GetRandomPokemonAsync(3);

            Assert.Equal(3, list.Count);

            foreach (var p in list)
            {
                Assert.NotEqual(0, p.Id);
                Assert.StartsWith("Pokemon", p.Name);
                Assert.Equal("primary", p.Type); // primary is slot 1
                Assert.EndsWith(".png", p.ImageUrl);
                Assert.True(p.Base_Experience > 0);
            }
        }

        [Fact]
        public async Task GetRandomPokemonAsync_ReturnsUniqueIds()
        {
            var client = CreateClient();
            var service = new PokemonService(client);

            var list = await service.GetRandomPokemonAsync(5);
            var distinct = list.Select(p => p.Id).Distinct().Count();

            Assert.Equal(5, distinct);
        }
    }
}
