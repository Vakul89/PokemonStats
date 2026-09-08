using Backend.Helpers;
using Backend.Models;

namespace UnitTest.Helpers
{
    public class PokemonBattleEngineTests
    {
        [Fact]
        public void RunRoundRobin_TypeAdvantage_P1Wins()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "Squirtle", Type = "water", Base_Experience = 50 };
            var p2 = new PokemonDTO { Id = 2, Name = "Charmander", Type = "fire", Base_Experience = 50 };

            var engine = new PokemonBattleEngine();
            engine.RunRoundRobin(new List<PokemonDTO> { p1, p2 });

            Assert.Equal(1, p1.Wins);
            Assert.Equal(0, p1.Losses);
            Assert.Equal(0, p1.Ties);

            Assert.Equal(0, p2.Wins);
            Assert.Equal(1, p2.Losses);
            Assert.Equal(0, p2.Ties);
        }

        [Fact]
        public void RunRoundRobin_TypeAdvantage_P2Wins()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "Bulbasaur", Type = "grass", Base_Experience = 60 };
            var p2 = new PokemonDTO { Id = 2, Name = "Vulpix", Type = "fire", Base_Experience = 10 };

            var engine = new PokemonBattleEngine();
            engine.RunRoundRobin(new List<PokemonDTO> { p1, p2 });

            // fire beats grass, so p2 should win despite lower experience
            Assert.Equal(0, p1.Wins);
            Assert.Equal(1, p1.Losses);
            Assert.Equal(0, p1.Ties);

            Assert.Equal(1, p2.Wins);
            Assert.Equal(0, p2.Losses);
            Assert.Equal(0, p2.Ties);
        }

        [Fact]
        public void RunRoundRobin_BaseExperienceDecides_WhenNoTypeAdvantage()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "Eevee", Type = "normal", Base_Experience = 200 };
            var p2 = new PokemonDTO { Id = 2, Name = "Pidgey", Type = "flying", Base_Experience = 50 };

            var engine = new PokemonBattleEngine();
            engine.RunRoundRobin(new List<PokemonDTO> { p1, p2 });

            Assert.Equal(1, p1.Wins);
            Assert.Equal(0, p1.Losses);
            Assert.Equal(0, p1.Ties);

            Assert.Equal(0, p2.Wins);
            Assert.Equal(1, p2.Losses);
            Assert.Equal(0, p2.Ties);
        }

        [Fact]
        public void RunRoundRobin_Tie_WhenEqualExperience_And_NoTypeAdvantage()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "A", Type = "normal", Base_Experience = 100 };
            var p2 = new PokemonDTO { Id = 2, Name = "B", Type = "normal", Base_Experience = 100 };

            var engine = new PokemonBattleEngine();
            engine.RunRoundRobin(new List<PokemonDTO> { p1, p2 });

            Assert.Equal(0, p1.Wins);
            Assert.Equal(0, p1.Losses);
            Assert.Equal(1, p1.Ties);

            Assert.Equal(0, p2.Wins);
            Assert.Equal(0, p2.Losses);
            Assert.Equal(1, p2.Ties);
        }

        [Fact]
        public void RunRoundRobin_MultiplePokemons_AllPairingsResolved()
        {
            var p1 = new PokemonDTO { Id = 1, Name = "Watermon", Type = "water", Base_Experience = 10 };
            var p2 = new PokemonDTO { Id = 2, Name = "Firemon", Type = "fire", Base_Experience = 100 };
            var p3 = new PokemonDTO { Id = 3, Name = "Grassmon", Type = "grass", Base_Experience = 30 };

            var list = new List<PokemonDTO> { p1, p2, p3 };
            var engine = new PokemonBattleEngine();
            engine.RunRoundRobin(list);

            // Pairings: (p1,p2), (p1,p3), (p2,p3)
            // p1 vs p2: water beats fire -> p1 wins
            // p1 vs p3: grass beats electric only; no advantage -> compare base exp (p3>p1) -> p3 wins
            // p2 vs p3: fire beats grass -> p2 wins

            Assert.Equal(1, p1.Wins); // beat p2
            Assert.Equal(1, p1.Losses); // lost to p3

            Assert.Equal(1, p2.Wins); // beat p3
            Assert.Equal(1, p2.Losses); // lost to p1

            Assert.Equal(1, p3.Wins); // beat p1
            Assert.Equal(1, p3.Losses); // lost to p2
        }
    }
}
