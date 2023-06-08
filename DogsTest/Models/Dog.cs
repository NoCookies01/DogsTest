using DogsTest.Contexts;

namespace DogsTest.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }
        public int Tail_Length { get; set; }
        public int Weight { get; set; }
    }
}
