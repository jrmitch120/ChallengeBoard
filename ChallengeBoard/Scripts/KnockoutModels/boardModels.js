/***************************************************************************************************************
* BoardListModel - A model for listing available challenge boards
***************************************************************************************************************/
function BoardListModel(boards) {
    var self = this;

    self.boards = ko.observableArray(boards);

    self.search = ko.observable();

    ko.computed(function () {
        if(self.search() != null)
            console.info('searching for:' + self.search());
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

