namespace RestApi.Models
{
    public class Borrower
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public ICollection<Book> BorrowedBooks { get; set; }
    }
}
