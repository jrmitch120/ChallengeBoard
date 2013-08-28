using System;
using System.Linq;
using System.Web.Mvc;
using ChallengeBoard.Infrastucture;
using ChallengeBoard.Models;
using ChallengeBoard.ViewModels;

using PagedList;

namespace ChallengeBoard.Controllers
{
    [Authorize]
    public class DiscussionController : Controller
    {
        private readonly IRepository _repository;

        public DiscussionController(IRepository repository)
        {
            _repository = repository;
        }

        //
        // GET: /Discussion/
        public ActionResult Index(int boardId, int? page)
        {
            var board = _repository.GetBoardById(boardId);
            var viewer = _repository.GetCompetitorByUserName(boardId, User.Identity.Name);
            var featured = true;

            if (board == null)
                return View("BoardNotFound");

            if (!page.HasValue)
            {
                var postCount = _repository.Posts.Count(p => p.Board.BoardId == boardId);

                if (postCount == 0)
                    page = 1;
                else
                {
                    page = postCount / PageLimits.Discussion;
                    if (postCount % PageLimits.Discussion != 0)
                        page++;
                }
            }
            else
                featured = false;

            var pagedPosts = _repository.Posts.Where(p => p.Board.BoardId == boardId)
                                        .OrderBy(p => p.Created)
                                        .ToPagedList(page.Value, PageLimits.Discussion);

            var focusPostId = featured ? pagedPosts.Max(p => p.PostId) : -1;

            return
                View(new DiscussionViewModel(board, pagedPosts.MapToViewModel(p => new PostViewModel(p)), focusPostId,
                                             viewer));
        }

        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Create(PostViewModel post)
        {
            var response = new JsonResponse<PostViewModel>();
            var newPost = new Post();

            try
            {
                newPost.Board = _repository.GetBoardById(post.BoardId);
                newPost.Owner = _repository.GetCompetitorByUserName(post.BoardId, User.Identity.Name);
                newPost.Body = post.Body;

                _repository.Add(newPost);
                _repository.CommitChanges();

                response.Result = new PostViewModel(newPost);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Error = true;
            }

            return (Json(response));
        }

        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Edit(PostViewModel post)
        {
            var response = new JsonResponse<string>();

            try
            {
                var editPost = _repository.GetPostById(post.PostId);

                if (!editPost.IsOwner(User.Identity.Name))
                    throw (new Exception("You are not the owner of this post"));

                editPost.Body = post.Body;
                editPost.Edited = DateTime.Now;

                _repository.CommitChanges();

                response.Result = String.Format("{0:g}", editPost.Edited);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Error = true;
            }

            return (Json(response));
        }

        [HttpPost]
        [Authorize]
        [AjaxOnly]
        public ActionResult Delete(int postId)
        {
            var response = new JsonResponse<PostViewModel>();

            try
            {
                var deletePost = _repository.GetPostById(postId);

                if (!deletePost.IsOwner(User.Identity.Name))
                    throw (new Exception("You are not the owner of this post"));

                _repository.Delete(deletePost);
                _repository.CommitChanges();
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Error = true;
            }

            return (Json(response));
        }
    }
}
