using Domain.Entities;

namespace Infrastructure.Data.Data
{
    public static class StateLGAData
    {
        public static List<State> GetStates()
        {
            return new List<State>
            {
                new State
                {
                    Id = 1,
                    Name = "Lagos"
                },
                new State
                {
                    Id = 2,
                    Name = "Anambra"
                }
            };
        }

        public static List<LGA> GetLGAs()
        {
            return new List<LGA>
            {
                new LGA { Id = 1, Name = "Eti-Osa", StateId = 1 },
                new LGA { Id = 2, Name = "Alimosho", StateId = 1 },
                new LGA { Id = 3, Name = "Aguata", StateId = 2 },
                new LGA { Id = 4, Name = "Anambra East", StateId = 2 }
            };
        }
        
    }
}
