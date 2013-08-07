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
    self.deleteRequested = ko.observable(false);
    
    self.originalBody = ko.observable();
    
    // Switch to edit mode.  Replace <br> and <a>
    self.setEditBody = function() {
        self.body(self.body().replace(/<br\s*[\/]?>/gi, '\n'));
        self.body(self.body().replace(/<a[^>]*>(.*?)<\/a>/ig, '$1'));
    };

    // Switch to HTML mode.  Replace line breaks and hotlink URLS
    self.setHtmlBody = function () {
        // Remove all tags.  More of a user interface nicety than a requirement.
        self.body(self.body().replace(/<([A-Z][A-Z0-9]*)[^>]*>(.*?)<\/\1>/ig, '$2'));
        
        self.body(self.body().replace(/\n/g, '<br />'));
        self.body(self.body().replace(/\b(https?):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|]/ig, '<a href="$&" target="_blank">$&</a>'));
    };   
};