namespace ChallengeBoard.Email
{
    public class EmailContact
    {
        public readonly string Name;
        public readonly string EmailAddress;

        public EmailContact(string name, string emailAddress)
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}