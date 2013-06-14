/*
    Unobstrusive hooks for custom validators.
*/
(function ($) {
    $.validator.unobtrusive.adapters.add(
        'isdatebefore', ['propertytested', 'allowequaldates'], function (options) {
            options.rules['isdatebefore'] = options.params;
            options.messages['isdatebefore'] = options.message;
        });

    $.validator.unobtrusive.adapters.add(
        'isdatewithinyear', [], function (options) {
            options.rules['isdatewithinyear'] = options.params;
            options.messages['isdatewithinyear'] = options.message;
        });

    $.validator.addMethod("isdatebefore", function (value, element, params) {
        var parts = element.name.split(".");
        var prefix = "";
        if (parts.length > 1)
            prefix = parts[0] + ".";
        var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();
        if (!value || !startdatevalue)
            return true;
        return (params.allowequaldates) ? Date.parse(startdatevalue) >= Date.parse(value) :
            Date.parse(startdatevalue) > Date.parse(value);
    });
    
    $.validator.addMethod("isdatewithinyear", function (value) {
        var d = new Date();

        d.setHours(0, 0, 0, 0);
        
        if (Date.parse(value) < d.getTime())
            return false;

        d.setFullYear(d.getFullYear() + 1);
        return (Date.parse(value) <= d.getTime());
    });

})(jQuery);