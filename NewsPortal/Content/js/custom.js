$(document).ready(function () {
    $(".add-comment").on("submit", function (event) {
        event.preventDefault();
        let action = $(this).attr("action");
        let method = $(this).attr("method");
        $.ajax({
            type: method,
            url: action,
            data: $(this).serialize(),
            success: function (data) {
                $(".comments-section .row").append(data);
                $(".add-comment")[0].reset();
                $(".add-comment-section form .row").append('<div class="col-md-12"><p class="comment-success">You have commented successfully !</p></div>');
            },
            error: function (data) {
                $(".add-comment-section form .row").append('<div class="col-md-12"><p class="comment-danger">Something went wrong.Please try again or contact our support !</p></div>');
            },
        });

    });
});