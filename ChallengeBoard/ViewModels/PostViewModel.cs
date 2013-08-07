using System;
using System.Text.RegularExpressions;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;

namespace ChallengeBoard.ViewModels
{
    public class PostViewModel
    {
        public int BoardId { get; set; }
        public int PostId { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string GravatarLink { get; set; }
        public string Body { get; set; }
        public string Created { get; set; }
        public string Edited { get; set; }

        public PostViewModel(){}

        public PostViewModel(Post post)
        {
            var g = new NGravatar.Gravatar();
            
            BoardId = post.Board.BoardId;
            PostId = post.PostId;
            OwnerId = post.Owner.CompetitorId;
            OwnerName = post.Owner.Name;
            GravatarLink = g.GetImageSource(post.Owner.Profile.EmailAddress);
            Body = HtmlSanitizer.Sanitizer(post.Body, new Regex("^(br|a)$"), HtmlSanitizer.ForbiddenTags).Val;
            Created = String.Format("{0:g}", post.Created);
            Edited = String.Format("{0:g}", post.Edited);
        }
    }
}