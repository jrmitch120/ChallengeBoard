/***************************************************************************************************************
* CompetitorModel - A model for a challenge board competitor
***************************************************************************************************************/
function CompetitorModel(data) {
    var self = this;

    self.statusOptions = ko.observable(data.Name + "<b class=\"caret\"></b>");
    self.name = data.Name;
    self.joined = data.Joined;
    self.gamesPlayed = data.GamesPlayed;
    self.rejectionsReceived = data.RejectionsReceived;
    self.rejectionsGiven = data.RejectionsGiven;
    self.status = ko.observable(data.Status);

    this.displayStatus = ko.computed(function () {
        switch (self.status()) {
            case 1:
                return ("Retired");
            case 2:
                return ("Banned");
            default:
                return ("Active");
        }
    }, this);
};