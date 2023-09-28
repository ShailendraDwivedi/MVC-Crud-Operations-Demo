// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
    var PlaceHolderElement = $("#PlaceHolderHere");
    $('button[data-toggle="ajax-modal"]').click(function (event) {

        var url = $(this).data('url');
        $.get(url).done(function (data) {
            PlaceHolderElement.html(data);
            PlaceHolderElement.find('.modal').modal('show');
        });
    });
    PlaceHolderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();
        $.post(actionUrl, dataToSend).done(function (data) {
            placeholderElement.find('.modal').modal('hide');
        });
    });
    $.post(actionUrl, dataToSend).done(function (data) {
        var newBody = $('.modal-body', data);
        placeholderElement.find('.modal-body').replaceWith(newBody);
    });
});