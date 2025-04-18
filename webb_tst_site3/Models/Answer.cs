namespace webb_tst_site3.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Score { get; set; } = 1;
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int ResultId { get; set; }
        public Result Result { get; set; }
    }
}
