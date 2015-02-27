/***************************************************************************************************************
* MatchModel - A model for a challenge board match
***************************************************************************************************************/
function MatchModel() {
    var self = this;

    self.winner = ko.observable(),
    self.winnerRatingDelta = ko.observable(),
    self.loser = ko.observable(),
    self.loserRatingDelta = ko.observable(),
    self.boardId = ko.observable(),
    self.matchId = ko.observable();
    self.tied = ko.observable();
    self.winnerComment = ko.observable();
    self.loserComment = ko.observable();
    self.action = ko.observable("");
    self.confirmation = ko.observable("test");

    self.actionDescription = ko.computed(function() {
        if (self.action().toLowerCase() === "withdraw")
            return "Type \"withdraw\" to remove this match.";
        else
            return "Type \"verify\" or \"reject\" to finalize this match.";
    });

    self.actionConfirmed = ko.computed(function () {
        
        if (self.action().toLowerCase() === "withdraw") {
            return self.confirmation().toLowerCase() === "withdraw";
        }

        return (self.confirmation().toLowerCase() === "reject" || self.confirmation().toLowerCase() === "verify");
    });
};