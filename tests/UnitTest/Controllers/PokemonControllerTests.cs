using Backend.Controllers;
using Backend.Helpers;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace UnitTest.Controllers
{
    class FakePokemonService : IPokemonService
    {
        private readonly List<PokemonDTO> _list;

        public FakePokemonService(List<PokemonDTO> list)
        {
            _list = list;
        }

        public Task<List<PokemonDTO>> GetRandomPokemonAsync(int pokemonFetchCount)
        {
            return Task.FromResult(_list);
        }
    }

    public class PokemonControllerTests
    {
        [Fact]
        public async Task GetPokemons_ReturnsBadRequest_When_SortByMissing()
        {
            var svc = new FakePokemonService(new List<PokemonDTO>());
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons(null!, "asc");

            Assert.IsType<BadRequestObjectResult>(result);
            var bad = result as BadRequestObjectResult;
            Assert.Equal("sortBy parameter is required.", bad!.Value);
        }

        [Fact]
        public async Task GetPokemons_ReturnsBadRequest_When_SortDirectionMissing()
        {
            var svc = new FakePokemonService(new List<PokemonDTO>());
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("id", null!);

            Assert.IsType<BadRequestObjectResult>(result);
            var bad = result as BadRequestObjectResult;
            Assert.Equal("sortDirection parameter is required.", bad!.Value);
        }

        [Fact]
        public async Task GetPokemons_ReturnsBadRequest_When_InvalidSortBy()
        {
            var svc = new FakePokemonService(new List<PokemonDTO>());
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("invalid", "asc");

            Assert.IsType<BadRequestObjectResult>(result);
            var bad = result as BadRequestObjectResult;
            Assert.Equal("sortBy parameter is invalid.", bad!.Value);
        }

        [Fact]
        public async Task GetPokemons_ReturnsBadRequest_When_InvalidSortDirection()
        {
            var svc = new FakePokemonService(new List<PokemonDTO>());
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("id", "up");

            Assert.IsType<BadRequestObjectResult>(result);
            var bad = result as BadRequestObjectResult;
            Assert.Equal("sortDirection parameter is invalid.", bad!.Value);
        }

        [Fact]
        public async Task GetPokemons_ReturnsSortedByWins_Asc()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "P1", Type = "water", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "P2", Type = "fire", Base_Experience = 20 };

            var svc = new FakePokemonService(new List<PokemonDTO> { p1, p2 });
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("wins", "asc");

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PokemonDTO>>(ok.Value);

            // p1 (water) beats p2 (fire) so p1 has 1 win, p2 has 0 -> asc order should place p2 first
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list[0].Id);
            Assert.Equal(1, list[1].Id);
        }

        [Fact]
        public async Task GetPokemons_ReturnsSortedByLosses_Asc()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "P1", Type = "water", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "P2", Type = "electric", Base_Experience = 20 };

            var svc = new FakePokemonService(new List<PokemonDTO> { p1, p2 });
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("losses", "asc");

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PokemonDTO>>(ok.Value);

            // p1 (water) beats p2 (electric) so p1 has 1 loss, p2 has 0 -> asc order should place p2 first
            Assert.Equal(2, list.Count);
            Assert.Equal(2, list[0].Id);
            Assert.Equal(1, list[1].Id);
        }

        [Fact]
        public async Task GetPokemons_ReturnsSortedByTies_Asc()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "P1", Type = "water", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "P2", Type = "water", Base_Experience = 20 };

            var svc = new FakePokemonService(new List<PokemonDTO> { p1, p2 });
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("ties", "asc");

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PokemonDTO>>(ok.Value);

            // p1 (water) beats p2 (water) so both have 0 losses, 0 wins, 1 tie -> asc order should place them in ID order
            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal(2, list[1].Id);
        }

        [Fact]
        public async Task GetPokemons_ReturnsSortedByName_Desc()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "Alpha", Type = "normal", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "Beta", Type = "normal", Base_Experience = 20 };

            var svc = new FakePokemonService(new List<PokemonDTO> { p1, p2 });
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("name", "desc");

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PokemonDTO>>(ok.Value);

            // Descending by name: Beta, Alpha
            Assert.Equal(2, list.Count);
            Assert.Equal("Beta", list[0].Name);
            Assert.Equal("Alpha", list[1].Name);
        }

        [Fact]
        public async Task GetPokemons_ReturnsSortedById_Asc()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "P1", Type = "water", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "P2", Type = "electric", Base_Experience = 20 };

            var svc = new FakePokemonService(new List<PokemonDTO> { p1, p2 });
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons("id", "asc");

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PokemonDTO>>(ok.Value);

            // asc order should place them in ID order
            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal(2, list[1].Id);
        }

        [Fact]
        public async Task GetPokemons_ReturnsSortedByDefault_Asc()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "P1", Type = "water", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "P2", Type = "electric", Base_Experience = 20 };

            var svc = new FakePokemonService(new List<PokemonDTO> { p1, p2 });
            var controller = new PokemonController(svc, new PokemonBattleEngine());

            var result = await controller.GetPokemons(null ?? "id", "asc");            

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<PokemonDTO>>(ok.Value);

            // asc order should place them in default order (sort is by ID)
            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal(2, list[1].Id);
        }
    }
}
