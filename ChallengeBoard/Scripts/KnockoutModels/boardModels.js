/***************************************************************************************************************
* BoardListModel - A model for listing available challenge boards
***************************************************************************************************************/
function BoardListModel(boards) {
    var self = this;

    self.initialLoad = boards;

    self.boards = ko.observableArray(boards);
    self.search = ko.observable();

    ko.computed(function () {
        if (self.search() != null) {
            if (self.search() == '')
                self.boards(self.initialLoad);
            else {
                var filter = '';
                if (window.location.search != '')
                    filter = '&' + window.location.search.substring(1);
                
                $.getJSON(window.location.pathname + "/Search?search=" + self.search() + filter, function (data) {
                    self.boards(data.Boards);
                });
            }
        }
    }).extend({ throttle: 500 });
}

/***************************************************************************************************************
* CompetitorStatusModel - A model for modifying a board's competitor (status only right now)
***************************************************************************************************************/
function CompetitorStatusModel(boardId, competitosData) {
    var self = this;

    self.boardId = boardId,
    self.competitors = new Array();
    
    for (var index in competitosData) {
        self.competitors.push(new CompetitorModel(competitosData[index]));
    }

    self.setStatus = function (status, competitor) {
        competitor.status(status);
        
        // Save competitor's status
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(competitor),
            url: '../competitors/' + self.boardId + '/edit/',
            success: function(data) {
                if (data.Error) {
                    alert(data.Message);
                }
            },
            error: function () {
                alert("There was a problem.  Please try again.");
            }
        });
    };
};

/***************************************************************************************************************
* DiscussionModel - A model for listing and creating posts
***************************************************************************************************************/
function DiscussionModel(boardId, viewer, postData, lastPage) {
    var self = this;

    self.boardId = boardId;
    
    // If we're on the last page, new posts will be tacked onto the "posts" collection.
    // Otherwise they will be tacked onto the "newPosts" collection, which is visually 
    // seperated from the main collection.
    self.lastPage = lastPage;
    
    self.newPost =
        new PostModel(
            {
                BoardId: self.boardId,
                PostId: 0, OwnerName: '',
                GravatarLink: '',
                Subject: '',
                Body: '',
                Created: '',
                Edited: ''
            });
    
    self.viewer = new CompetitorModel(viewer);
    
    self.posts = ko.observableArray();
    self.newPosts = ko.observableArray();

    for (var index in postData) {
        self.posts.push(new PostModel(postData[index]));
    }

    self.createPost = function () {
        self.newPost.setHtmlBody();
        $('#post').addClass('disabled');
        $('#post').attr('disabled', 'disabled');
        
        // Create a new post
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(self.newPost),
            url: self.boardId + '/create/',
            success: function (data) {
                if (data.Error)
                    alert(data.Message);
                else {
                    self.newPost.body('');
                    
                    if(lastPage) // Last page?  Tack onto the existing posts.  Otherwise the "newPosts" collection
                        self.posts.push(new PostModel(data.Result));
                    else
                        self.newPosts.push(new PostModel(data.Result));
                    
                    var aPost = $('#post-' + data.Result.PostId);
                    var aHeader = $('.navbar-fixed-top:first');

                    $('html,body').animate({ scrollTop: aPost.offset().top - aHeader.height() }, 'slow');
                }
            },
            error: function () {
                alert("There was a problem.  Please try again.");
            },
            complete: function () {
                $('#post').removeClass('disabled');
                $('#post').removeAttr('disabled');
            }
        });
    };

    self.cancelPost = function (post) {
        post.body(post.originalBody());
        post.editing(false);
    };
    
    
    self.editPost = function (post) {
        post.originalBody(post.body());
        post.setLinebreakBody();
        post.editing(true);
        $('#editPost-' + post.postId()).focus();
        /* Half ass way to get the cursor to the end of the textarea.  
            var val = post.body();
            post.body('');
        
            setTimeout(function () {
                post.body(val);
            }, 1);
        */
    };
    
    self.savePost = function (post) {
        post.setHtmlBody();
        post.editing(false);
        
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            data: ko.toJSON(post),
            url: self.boardId + '/edit/' + post.postId(),
            success: function (data) {
                if (data.Error)
                    alert(data.Message);
                else {
                    post.edited(data.Result);
                    self.flashMessage($('#post-' + post.postId() + '-msg'), 'message updated');
                }
            },
            error: function () {
                alert("There was a problem.  Please try again.");
            },
        });
    };
    
    self.deletePost = function (post) {
        post.editing(false);
        
        // Delete a post
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            url: self.boardId + '/delete/' + post.postId(),
            success: function(data) {
                if (data.Error)
                    alert(data.Message);
                else {
                    $('#post-' + post.postId()).slideUp('fast', function () {
                        if (self.posts.remove(post).length == 0) 
                            self.newPosts.remove(post);
                    });
                }
            },
            error: function() {
                alert("There was a problem.  Please try again.");
            },
            complete: function() {
                $('#post').removeClass('disabled');
                $('#post').removeAttr('disabled');
            }
        });
    };

    self.flashMessage = function (element, message) {
        element.html(message).fadeIn('fast').delay(500).fadeOut('fast');
    };
}