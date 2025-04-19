function addToWishlist(productId, productVariantId) {
    $.ajax({
        url: '@Url.Action("AddToWishlist", "Profile")',
        type: 'POST',
        data: {
            productId: productId,
            productVariantId: productVariantId
        },
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            if (data.success) {
                showMessage('Product Added Successfully!', 'success');
            } else if (data.loginRequired) {
                window.location.href = '@Url.Action("Login", "Account")';
            }
        },
        error: function () {
            showMessage('Error adding product to wishlist', 'error');
        }
    });
}

// Message display function
function showMessage(text, type) {
    const $message = $('.message');
    $message.text(text)
        .removeClass('error success')
        .addClass(type)
        .fadeIn(300)
        .delay(2000)
        .fadeOut(300);
}