namespace ChallengeBoard.Models
{
    public class JsonResponse<T>
    {
        public bool Error { get; set; }
        public string Message { get; set;}
        public T Result { get; set; } 
    }
}