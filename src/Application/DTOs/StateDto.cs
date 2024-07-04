
namespace Application.DTOs
{
    public class StateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class LGADto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
    }
}

