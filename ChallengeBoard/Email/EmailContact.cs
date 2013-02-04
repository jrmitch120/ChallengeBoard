namespace ChallengeBoard.Email
{
    public class EmailContact
    {
        public readonly string Name;
        public readonly string EmailAddress;

        public EmailContact(string emailAddress, string name)
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}