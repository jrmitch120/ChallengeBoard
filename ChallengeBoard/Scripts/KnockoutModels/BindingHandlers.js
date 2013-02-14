ko.bindingHandlers.date = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var jsonDate = valueAccessor();
        var value = new Date(parseInt(jsonDate.substr(6)));
        var ret = ('0' + (value.getMonth() + 1)).slice(-2) + '/' + ('0' + value.getDate()).slice(-2) + '/' + value.getFullYear();
        element.innerHTML = ret;
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {

    }
};