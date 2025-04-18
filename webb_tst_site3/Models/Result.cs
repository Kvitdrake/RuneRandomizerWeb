namespace webb_tst_site3.Models
{
    public class Result
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
