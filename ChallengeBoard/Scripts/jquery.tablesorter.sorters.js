$.tablesorter.addParser({
    // set a unique id 
    id: 'eloChange',
    is: function (s) {
        // return false so this parser is not auto detected 
        return false;
    },
    format: function (s, table, cell) {
        // format your data for normalization 
        console.info($(cell).children("span").text());
        return $(cell).children("span").text();
    },
    // set type, either numeric or text 
    type: 'numeric'
});