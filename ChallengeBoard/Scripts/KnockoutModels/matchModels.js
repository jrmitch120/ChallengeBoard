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
};