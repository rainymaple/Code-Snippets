function getRollbackStatus(method) {
    enableRollback(false);
    jQuery.ajax({
        type: 'GET',
        cache: false,
        url: 'Connector.ashx',
        data: 'MethodName=' + method
    })
        .done(function (data) {
            var enable = data.data;
            enableRollback(enable);
        }).fail(function (data) {
            enableRollback(false);
        }).always(function (date) {
        });
}