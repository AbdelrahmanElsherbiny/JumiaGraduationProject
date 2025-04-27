var itemIdToDelete = null;

$(document).ready(function () {
    $(".remove-btn").click(function () {
        itemIdToDelete = $(this).data("id");
        $("#deleteConfirmModal").modal("show");
    });

    $("#confirmDeleteBtn").click(function () {
        if (itemIdToDelete !== null) {
            $.ajax({
                type: "POST",
                url: "/Cart/RemoveCartItemAjax",
                data: { itemId: itemIdToDelete },
                success: function (response) {
                    if (response.success) {
                        $("#deleteConfirmModal").modal("hide");

                        $("#alertMessage").removeClass("d-none").fadeIn();

                        setTimeout(function () {
                            $("#alertMessage").fadeOut();
                            location.reload();
                        }, 2000);
                    } else {

                        $("#alertMessageFail").removeClass("d-none").fadeIn();

                        setTimeout(function () {
                            $("#alertMessageFail").fadeOut();
                            location.reload();
                        }, 2000);
                    }
                }
            });
        }
    });
});
function updateCartTotalQuantity() {
    $.ajax({
        url: '/Cart/GetTotalCartQuantity',
        type: 'GET',
        success: function (totalQuantity) {
            $('.cart-total-quantity').text(`Cart (${totalQuantity})`);
            $('.cart-total-quantity2').text(`Item's total (${totalQuantity})`);
        },
        error: function (xhr, status, error) {
            console.error("Error updating total cart quantity:", error);
        }
    });
}
function updateCartTotalPrice() {
    $.ajax({
        url: '/Cart/GetTotalPrice',
        type: 'GET',
        success: function (totalPrice) {
            $('.cart-total-price').text(`EGP ${totalPrice}`);
        },
        error: function (xhr, status, error) {
            console.error("Error updating total cart quantity:", error);
        }
    });
}

$(document).ready(function () {
    updateCartTotalQuantity();
    updateCartTotalPrice();
    var selectedProductId = null;
    var selectedVariantId = null;

    $('.add-to-cart-btn').on('click', function () {
        selectedProductId = $(this).data('product-id');
        var hasVariant = $(this).data('has-variant'); 

        if (hasVariant === true || hasVariant === "true") {
            
            $.ajax({
                url: '/Cart/GetVariants',
                data: { productId: selectedProductId },
                method: 'GET',
                success: function (data) {
                    $('#variantOptions').empty();       
                    $.each(data, function (index, variant) {
                        $('#variantOptions').append(`
                            <div class="variant-row d-flex justify-content-between align-items-center mb-2 p-2" data-variant-id="${variant.variantId}">
                                <div class="variant-details">
                                    <h6 class="m-0">${variant.sizeName}</h6>
                                    <span class="current-price">EGP ${(variant.price - (variant.price * variant.discount))}</span>
                                    <span class="original-price text-muted text-decoration-line-through">EGP ${variant.price}</span>
                                    <p class="m-0 text-warning">${variant.stock} units left</p>
                                </div>
                                <div class="quantity-controls d-flex align-items-center">
                                    <button class="quantity-btn minus-btn border border-0 rounded quantity-btn me-4 shadow-sm" data-variant-id="${variant.variantId}" data-stock="${variant.stock}" disabled>−</button>
                                    <span class="quantity-display mx-2" data-quantity="0">0</span>
                                    <button class="quantity-btn plus-btn border border-0 rounded quantity-btn ms-4 shadow-sm" data-variant-id="${variant.variantId}" data-stock="${variant.stock}">+</button>
                                </div>
                            </div>
                        `);
                    });

                   
                    $('#variantModal').modal('show');
                },
                error: function () {
                    alert('Error retrieving variants.');
                }
            });
        } else {
            
            selectedProductId = $(this).data('product-id');
            selectedVariantId = null;
            
            $.ajax({
                url: '/Cart/GetCartItemQuantity',
                type: 'GET',
                data: {
                    productId: selectedProductId,
                    variantId: null
                },
                success: function (existingQuantity) {
                 
                    var newQuantity = existingQuantity + 1;
                  
                    $.ajax({
                        url: '/Cart/AddOrUpdateCartItem',
                        type: 'POST',
                        data: {
                            productId: selectedProductId,
                            variantId: selectedVariantId,
                            quantity: newQuantity
                        },
                        success: function () {
                            $("#alertMessageAddtoCart").removeClass("d-none").fadeIn();
                            setTimeout(function () {
                                $("#alertMessageAddtoCart").fadeOut();
                            }, 2000);
                            updateCartTotalQuantity();
                            updateCartTotalPrice();
                            location.reload();
                        },
                        error: function (xhr) {
                            $("#alertMessageFailToAdd").removeClass("d-none").fadeIn();
                            setTimeout(function () {
                                $("#alertMessageFailToAdd").fadeOut();
                            }, 2000);
                            updateCartTotalQuantity();
                            updateCartTotalPrice();
                        }
                    });
                }
            });
        }
    }); 
    $(document).on('click', '.plus-btn', function () {
        const $row = $(this).closest('.variant-row');
        const $quantityDisplay = $row.find('.quantity-display');
        const $minusBtn = $row.find('.minus-btn');
        const stock = parseInt($(this).data('stock'));
        let quantity = parseInt($quantityDisplay.data('quantity'));
        const variantId = $(this).data('variant-id');

        if (quantity < stock) {
            quantity++;
            $quantityDisplay.data('quantity', quantity).text(quantity);
            $minusBtn.prop('disabled', false);
            if (quantity >= stock) {
                $(this).prop('disabled', true);
            }
            $.ajax({
                url: '/Cart/AddOrUpdateCartItem',
                method: 'POST',
                data: {
                    productId: selectedProductId,
                    variantId: variantId,
                    quantity: quantity
                },
                success: function () {
                    $("#alertMessageAddtoCart").removeClass("d-none").fadeIn();
                    setTimeout(function () {
                        $("#alertMessageAddtoCart").fadeOut();
                    }, 2000);
                    updateCartTotalQuantity();
                    updateCartTotalPrice();
                    location.reload();
                },
                error: function () {
                    quantity--;
                    $quantityDisplay.data('quantity', quantity).text(quantity);
                    $minusBtn.prop('disabled', quantity <= 0);
                    $(this).prop('disabled', quantity >= stock);
                    $("#alertMessageFailToAdd").removeClass("d-none").fadeIn();
                    setTimeout(function () {
                        $("#alertMessageFailToAdd").fadeOut();
                    }, 2000);
                    updateCartTotalQuantity();
                    updateCartTotalPrice();
                }
            });
        }
    });

    // Handle "−" button in modal (decrement and update cart)
    $(document).on('click', '.minus-btn', function () {
        const $row = $(this).closest('.variant-row');
        const $quantityDisplay = $row.find('.quantity-display');
        const $plusBtn = $row.find('.plus-btn');
        let quantity = parseInt($quantityDisplay.data('quantity'));
        const variantId = $(this).data('variant-id');

        if (quantity > 0) {
            quantity--;
            $quantityDisplay.data('quantity', quantity).text(quantity);
            $plusBtn.prop('disabled', false);
            if (quantity <= 0) {
                $(this).prop('disabled', true);
            }
            $.ajax({
                url: '/Cart/AddOrUpdateCartItem',
                method: 'POST',
                data: {
                    productId: selectedProductId,
                    variantId: variantId,
                    quantity: quantity
                },
                success: function () {
                    $("#alertMessageUpdateCart").removeClass("d-none").fadeIn();
                    setTimeout(function () {
                        $("#alertMessageUpdateCart").fadeOut();
                    }, 2000);
                    updateCartTotalQuantity();
                    updateCartTotalPrice();
                },
                error: function () {
                    quantity++;
                    $quantityDisplay.data('quantity', quantity).text(quantity);
                    $plusBtn.prop('disabled', false);
                    $(this).prop('disabled', quantity <= 0);
                    $("#alertMessageFailUpdate").removeClass("d-none").fadeIn();
                    setTimeout(function () {
                        $("#alertMessageFailUpdate").fadeOut();
                    }, 2000);
                    updateCartTotalQuantity();
                    updateCartTotalPrice();
                }
            });
        }
    });

    // Handle "+" button on cart page (increment and update cart)
    $(document).on('click', '.cart-plus-btn', function () {
        const $row = $(this).closest('.cart-item-row');
        const $quantityDisplay = $row.find('.quantity-display');
        const $minusBtn = $row.find('.cart-minus-btn');
        const stock = parseInt($(this).data('stock'));
        const stocknovariant = parseInt($(this).data('novariantstock'));
        
        const productId = $(this).data('product-id');
        let variantId = $(this).data('variant-id');
        if (!variantId || variantId === "" || variantId === "null" || variantId==null) {
            $.ajax({
                url: '/Cart/GetCartItemQuantity',
                type: 'GET',
                data: {
                    productId: productId,
                    variantId: null
                },
                success: function (existingQuantity) {
                    let newQuantity = existingQuantity + 1;
                    if (newQuantity <= stocknovariant) {
                        $quantityDisplay.text(newQuantity);
                        $minusBtn.prop('disabled', false);
                        if (newQuantity >= stocknovariant) {
                            $(this).prop('disabled', true);
                    
                        }

                      
                        $.ajax({
                            url: '/Cart/AddOrUpdateCartItem',
                            type: 'POST',
                            data: {
                                productId: productId,
                                variantId: null,
                                quantity: newQuantity
                            },
                            success: function () {
                                $("#alertMessageAddtoCart").removeClass("d-none").fadeIn();
                                setTimeout(function () {
                                    $("#alertMessageAddtoCart").fadeOut();
                                }, 2000);
                                updateCartTotalQuantity();
                                updateCartTotalPrice();
                            },
                            error: function (xhr) {
                                $("#alertMessageFailToAdd").removeClass("d-none").fadeIn();
                                setTimeout(function () {
                                    $("#alertMessageFailToAdd").fadeOut();
                                }, 2000);
                                updateCartTotalQuantity();
                                updateCartTotalPrice();
                            }
                        });
                    }
                }
            });
        } else {
            $.ajax({
                url: '/Cart/GetCartItemQuantity',
                method: 'GET',
                data: {
                    productId: productId,
                    variantId: variantId
                },
                success: function (currentQuantity) {
                    console.log("roma before ajax");
                    let newQuantity = currentQuantity + 1;
                    if (newQuantity <= stock) {
                        $quantityDisplay.text(newQuantity);
                        $minusBtn.prop('disabled', false);
                        if (newQuantity >= stock) {
                            $(this).prop('disabled', true);
                        }
                        $.ajax({
                            url: '/Cart/AddOrUpdateCartItem',
                            method: 'POST',
                            data: {
                                productId: productId,
                                variantId: variantId,
                                quantity: newQuantity
                            },
                            success: function () {
                                $("#alertMessageUpdateCart").removeClass("d-none").fadeIn();
                                setTimeout(function () {
                                    $("#alertMessageUpdateCart").fadeOut();
                                }, 2000);
                                updateCartTotalQuantity();
                                updateCartTotalPrice();
                            },
                            error: function () {
                                $quantityDisplay.text(currentQuantity);
                                $("#alertMessageFailUpdate").removeClass("d-none").fadeIn();
                                setTimeout(function () {
                                    $("#alertMessageFailUpdate").fadeOut();
                                }, 2000);
                                updateCartTotalQuantity();
                                updateCartTotalPrice();
                            }
                        });
                    }
                }
            });
        }
    });

    $(document).on('click', '.cart-minus-btn', function () {
        const $row = $(this).closest('.cart-item-row');
        const $quantityDisplay = $row.find('.quantity-display');
        const $plusBtn = $row.find('.cart-plus-btn');
        const productId = $(this).data('product-id');
        const variantId = $(this).data('variant-id');

        $.ajax({
            url: '/Cart/GetCartItemQuantity',
            method: 'GET',
            data: {
                productId: productId,
                variantId: variantId
            },
            success: function (currentQuantity) {
                let newQuantity = currentQuantity - 1;
                if (newQuantity >= 1) {
                    $quantityDisplay.text(newQuantity);
                    $plusBtn.prop('disabled', false);
                    if (newQuantity <= 1) {
                        $(this).prop('disabled', true);
                    }

                   
                    $.ajax({
                        url: '/Cart/AddOrUpdateCartItem',
                        method: 'POST',
                        data: {
                            productId: productId,
                            variantId: variantId,
                            quantity: newQuantity
                        },
                        success: function () {
                            $("#alertMessageUpdateCart").removeClass("d-none").fadeIn();
                            setTimeout(function () {
                                $("#alertMessageUpdateCart").fadeOut();
                            }, 2000);
                            updateCartTotalQuantity();
                            updateCartTotalPrice();
                        },
                        error: function () {
                            $quantityDisplay.text(currentQuantity);
                            $("#alertMessageFailUpdate").removeClass("d-none").fadeIn();
                            setTimeout(function () {
                                $("#alertMessageFailUpdate").fadeOut();
                            }, 2000);
                            updateCartTotalQuantity();
                            updateCartTotalPrice();
                        }
                    });
                }
            }
        });
    });
});

