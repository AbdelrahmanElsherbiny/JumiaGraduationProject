var itemIdToDelete = null;

function AddToWishlist(productId, varientId) {
    let selectedProductId = productId;
    let selectedVariantId = varientId;

    $.ajax({
        url: '/Profile/ToggleWishlist',
        type: 'Post',
        data: {
            productId: selectedProductId,
            productVariantId: selectedVariantId
        },
        success: function (response) {
            if (response.result == true) {
                $("#alertMessageAddtoWishlist").removeClass("d-none").fadeIn();
                setTimeout(function () {
                    $("#alertMessageAddtoWishlist").fadeOut();
                }, 2000);
            } else {
                $("#alertMessageRemoveWishlist").removeClass("d-none").fadeIn();
                setTimeout(function () {
                    $("#alertMessageRemoveWishlist").fadeOut();
                }, 2000);
            }

        },
        error: function (xhr) {
            $("#alertMessageFailToAdd").removeClass("d-none").fadeIn();
            setTimeout(function () {
                $("#alertMessageFailToAdd").fadeOut();
            }, 2000);
        }
    });
}
$(document).ready(function () {
    var selectedProductId = null;
    var selectedVariantId = null;

    $('.add-to-wishlist-btn').on('click', function () {
        selectedProductId = $(this).data('product-id');
        var hasVariant = $(this).data('has-variant');

        if (hasVariant === true || hasVariant === "true") {

            $.ajax({
                url: '/Cart/GetVariants',
                data: { productId: selectedProductId },
                method: 'GET',
                success: function (data) {
                    $('#variantOptions1').empty();
                    $.each(data, function (index, variant) {
                        $('#variantOptions1').append(`
                            <div class="variant-row" data-variant-id="${variant.variantId}">
                                <div class="variant-details border rounded bg-transparent">
                                    <Button class="m-0 size-btn bg-transparent border border-1 rounded px-2 py-1 add-to-wishlist" onclick="AddToWishlist(${selectedProductId},${variant.variantId})">${variant.sizeName}</Button>
                                </div>
                            </div>
                        `);
                    });


                    $('#variantModal1').modal('show');
                },
                error: function () {
                    alert('Error retrieving variants.');
                }
            });
        } else {

            selectedProductId = $(this).data('product-id');
            selectedVariantId = null;

            $.ajax({
                url: '/Profile/ToggleWishlist',
                type: 'Post',
                data: {
                    productId: selectedProductId,
                    variantId: null
                },
                success: function (response) {
                    if (response.result == true) {
                        $("#alertMessageAddtoWishlist").removeClass("d-none").fadeIn();
                        setTimeout(function () {
                            $("#alertMessageAddtoWishlist").fadeOut();
                        }, 2000);
                    } else {
                        $("#alertMessageRemoveWishlist").removeClass("d-none").fadeIn();
                        setTimeout(function () {
                            $("#alertMessageRemoveWishlist").fadeOut();
                        }, 2000);
                    }
                },
                error: function (xhr) {
                    $("#alertMessageFailToAdd").removeClass("d-none").fadeIn();
                    setTimeout(function () {
                        $("#alertMessageFailToAdd").fadeOut();
                    }, 2000);
                }
            });
        }
    });

})