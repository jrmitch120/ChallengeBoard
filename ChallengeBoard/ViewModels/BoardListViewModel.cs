using System.Collections.Generic;
using System.Linq;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class BoardListViewModel
    {
        public IEnumerable<BoardViewModel> Boards { get; set; }

        public BoardListViewModel(IEnumerable<Board> boards)
        {
            Boards = boards.Select(board => new BoardViewModel(board)).ToList();
        }
    }
}