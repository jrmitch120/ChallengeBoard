using System;

namespace ChallengeBoard.Services
{
    public class ServiceException : Exception 
    {
        public ServiceException(string message) : base(message) { }
    }
}