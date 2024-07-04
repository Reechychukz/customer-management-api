namespace Domain.Entities
{
    public class LGA
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StateId { get; set; }
        public State State { get; set; }
        public ICollection<User> Users { get; set; }
    }
}

