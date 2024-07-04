namespace Domain.Entities
{
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<LGA> LGAs { get; set; }
        public ICollection<User> Users { get; set; }
    }
}

