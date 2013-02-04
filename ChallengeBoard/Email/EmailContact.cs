namespace ChallengeBoard.Email
{
    public class EmailContact
    {
        public readonly string EmailAddress ;
        public readonly string Name;

        public EmailContact(string emailAddress, string name)
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}