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

            var jumpToLastRead = true;

            if (board == null)
                return View("BoardNotFound");

            if(viewer == null)
                return View(new DiscussionViewModel(board, null, 0, null));

            if (!page.HasValue)
            {
                var postCount = _repository.Posts.Count(p => p.Board.BoardId == boardId);
                var unreadCount =
                    _repository.Posts.Count(p => p.Board.BoardId == boardId && p.PostId > viewer.LastViewedPostId);
                var readCount = postCount - unreadCount;

                page = readCount == 0 ? 1 : 1 + (readCount/PageLimits.Discussion);
            }
            else
                jumpToLastRead = false;
            
            // Get our paged posts
            var pagedPosts = _repository.Posts.Where(p => p.Board.BoardId == boardId)
                                        .OrderBy(p => p.Created)
                                        .ToPagedList(page.Value, PageLimits.Discussion);
            
            // Set our focus post id
            var focusPostId = jumpToLastRead && pagedPosts.Any()
                                  ? pagedPosts.Where(p => p.PostId > viewer.LastViewedPostId).Min(p => (int?)p.PostId) ??
                                    viewer.LastViewedPostId
                                  : -1;

            // Update our last viewed post if it's more recent than what's being viewed
            if (pagedPosts.Any() && viewer.LastViewedPostId < pagedPosts.Max(p => p.PostId))
            {
                viewer.LastViewedPostId = pagedPosts.Max(p => p.PostId);
                _repository.CommitChanges();
            }

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

                newPost.Owner.LastViewedPostId = newPost.PostId;
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
