namespace ChallengeBoard.Infrastucture
{
    public class PageHeader
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }

        public PageHeader(string heading)
        {
            Heading = heading;
        }
        public PageHeader(string heading, string subHeading)
        {
            Heading = heading;
            SubHeading = subHeading;
        }
    }
}