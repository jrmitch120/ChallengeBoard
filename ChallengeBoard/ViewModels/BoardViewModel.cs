using System;
using System.ComponentModel.DataAnnotations;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class BoardViewModel
    {
        public int BoardId { get; set; }

        [Display(Name = "Board Name")]
        public string Name { get; set; }

        public CompetitorViewModel Owner { get; set; }
        
        [Display(Name = "Created")]
        public string Created { get; set; }

        [Display(Name = "Ends")]
        public string End { get; set; }

        [Display(Name = "Progress")]
        public int PercentComplete { get; set; }

        public BoardViewModel(Board board)
        {
            BoardId = board.BoardId;
            Name = board.Name;
            Owner = new CompetitorViewModel(board.Owner);
            Created = String.Format("{0:g}", board.Created);
            End = String.Format("{0:g}", board.End);

            PercentComplete = CalculatePercentComplete(board);
        }

        private static int CalculatePercentComplete(Board board)
        {
            if (board.Started == DateTime.MinValue)
                return (0);
            if (DateTime.Now >= board.End)
                return(100);
        
            var totalDays = (board.End - board.Started).TotalDays;
            var elapsedDays = totalDays - (board.End - DateTime.Now).TotalDays;
            return ((elapsedDays / totalDays * 100).RoundToWhole());
        }   
    }
}