$(function () {
    // Popovers
    $("a[rel='popover']").popover().on('click', function(e) {
        e.preventDefault();
        return true;
    });
    
    // Tooptips
    $("a[rel='tooltip']").tooltip().on('click', function (e) {
        e.preventDefault();
        return true;
    });
    
    // Rich text editors
    $('.editor').wysihtml5({
        "html": true, //(Un)ordered lists, e.g. Bullets, Numbers. Default true
    });
});