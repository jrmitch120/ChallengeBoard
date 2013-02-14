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
            url: '../Competitors/Edit/' + self.boardId + '/' + competitor.name,
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
