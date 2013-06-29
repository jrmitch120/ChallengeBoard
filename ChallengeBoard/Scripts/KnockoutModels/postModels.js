/***************************************************************************************************************
* PostModel - A model for a challenge board post
***************************************************************************************************************/
function PostModel(data) {
    var self = this;

    self.boardId = ko.observable(data.boardId);
    self.postId = ko.observable(data.PostId);
    self.postedBy = ko.observable(data.OwnerName),
    self.postedById = ko.observable(data.OwnerId),
    self.gravatarLink = ko.observable(data.GravatarLink),
    self.body = ko.observable(data.Body),
    self.created = ko.observable(data.Created),
    self.edited = ko.observable(data.Edited);
    
    self.editing = ko.observable(false);
    self.originalBody = ko.observable();

    self.setLinebreakBody = function() {
        self.body(self.body().replace(/<br\s*[\/]?>/gi, '\n'));
    };

    self.setHtmlBody = function() {
        self.body(self.body().replace(/\n/g, '<br />'));
    };
};