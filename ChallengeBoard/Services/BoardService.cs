using System;
using System.Linq;
using ChallengeBoard.Email;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Scoring;
using ChallengeBoard.Models;

namespace ChallengeBoard.Services
{
    public class BoardService : IBoardService
    {

        public void AdjustMatchDeadlines(Board board)
        {
            foreach (var match in board.Matches.Where(match => !match.IsResolved))
            {
                match.VerificationDeadline = match.Created.AddHours(board.AutoVerification);
            }
        }
    }
}