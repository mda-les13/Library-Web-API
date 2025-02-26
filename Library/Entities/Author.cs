using Library.DataAccess.Entities;

namespace Library.DataAccess.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
