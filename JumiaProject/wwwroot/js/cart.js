//////////////////333333333333333333333333333third updated one the perfect on until now//////////////////
function updateTotalQuantity(productId) {
 
        // Original AJAX call as fallback
        $.ajax({
            url: '/Cart/GetTotalProductQuantity',
            type: 'GET',
            data: { productId: productId },
            success: function (totalQuantity) {
                $('#quantitySpan_' + productId).text(Number(totalQuantity));
            },
            error: function () {
                console.error('Failed to fetch total quantity');
            }
        });
    
}
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
            console.error("Error updating total cart price:", error);
        }
    });
}

function updateCartItemUI(productId, variantId, quantity) {
    // Find the display element for this product/variant
    let selector;
    if (variantId) {
        selector = `.quantity-display[data-product-id="${productId}"][data-variant-id="${variantId}"]`;
    } else {
        selector = `.quantity-display[data-product-id="${productId}"][data-variant-id="null"]`;
    }
   
    // Update the display
    const $display = $(selector);
    if ($display.length) {
        //updateTotalQuantity(productId);   ///roma 
        $display.text(quantity);
        $display.data('quantity', quantity);
    } else {
        console.warn(`No quantity display found for product ${productId}, variant ${variantId}`);
    }
}

function initializeCartItems() {
    // Initialize quantities from database for all displayed cart items
    $('.cart-item-row').each(function () {
        const $row = $(this);
        const productId = $row.data('product-id');
        const variantId = $row.data('variant-id') === "null" ? null : $row.data('variant-id');
        const $quantityDisplay = $row.find('.quantity-display');

        if (productId) {
            //console.log(`Initializing cart item: product=${productId}, variant=${variantId}`);

            $.ajax({
                url: '/Cart/GetCartItemQuantity',
                type: 'GET',
                data: {
                    productId: productId,
                    variantId: variantId
                },
                success: function (quantity) {
                    console.log(`Retrieved quantity: ${quantity} for product=${productId}, variant=${variantId}`);
                    $quantityDisplay.text(quantity);
                    $quantityDisplay.data('quantity', quantity);
                    updateTotalQuantity(productId);
                },
                error: function (xhr, status, error) {
                    console.error(`Error getting quantity for product=${productId}, variant=${variantId}:`, error);
                }
            });
        } else {
            console.warn("Found cart-item-row without product-id attribute");
        }
    });
}

$(document).ready(function () {
    // Initialize cart counts and prices
    updateCartTotalQuantity();
    updateCartTotalPrice();

    // Initialize all cart items with correct quantities
    initializeCartItems();
    $('.cart-item-row').each(function () {
        const productId = $(this).data('product-id');
        updateTotalQuantity(productId);
    });
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
                        // Get current quantity for this variant from cart
                        $.ajax({
                            url: '/Cart/GetCartItemQuantity',
                            type: 'GET',
                            async: false, // Important to ensure the quantity is set before displaying
                            data: {
                                productId: selectedProductId,
                                variantId: variant.variantId
                            },
                            success: function (existingQuantity) {
                                // Now use the existingQuantity from database
                                const minusBtnDisabled = existingQuantity <= 0 ? 'disabled' : '';
                                const plusBtnDisabled = existingQuantity >= variant.stock ? 'disabled' : '';

                                $('#variantOptions').append(`
                                    <div class="variant-row d-flex justify-content-between align-items-center mb-2 p-2" data-variant-id="${variant.variantId}">
                                        <div class="variant-details">
                                            <h6 class="m-0">${variant.sizeName}</h6>
                                            <span class="current-price">EGP ${(variant.price - (variant.price * variant.discount))}</span>
                                            <span class="original-price text-muted text-decoration-line-through">EGP ${variant.price}</span>
                                            <p class="m-0 text-warning">${variant.stock} units left</p>
                                        </div>
                                        <div class="quantity-controls d-flex align-items-center">
                                            <button class="quantity-btn minus-btn border border-0 rounded quantity-btn me-4 shadow-sm"
                                                data-variant-id="${variant.variantId}"
                                                data-stock="${variant.stock}"
                                                data-product-id="${selectedProductId}"
                                                ${minusBtnDisabled}>−</button>
                                            <span class="quantity-display mx-2"
                                                data-quantity="${existingQuantity}"
                                                data-product-id="${selectedProductId}"
                                                data-variant-id="${variant.variantId}">${variant.quantityInCart}</span>
                                            <button class="quantity-btn plus-btn border border-0 rounded quantity-btn ms-4 shadow-sm"
                                                data-variant-id="${variant.variantId}"
                                                data-stock="${variant.stock}"
                                                data-product-id="${selectedProductId}"
                                                ${plusBtnDisabled}>+</button>
                                        </div>
                                    </div>
                                `);
                            }
                        });
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
                                updateCartTotalQuantity();
                                updateCartTotalPrice();
                                //console.log("selectedProductId"+ selectedProductId);
                                // Update UI without page reload
                                updateCartItemUI(selectedProductId, null, newQuantity);
                                updateTotalQuantity(selectedProductId);    ////////roma new
                                //console.log("selectedProductId" + selectedProductId);
                            }, 2000);
                            location.reload();
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
        }
    });

//
$(document).on('click', '.plus-btn', function () {
    const $row = $(this).closest('.variant-row');
    const $quantityDisplay = $row.find('.quantity-display');
    const $minusBtn = $row.find('.minus-btn');
    const stock = parseInt($(this).data('stock'));
    let quantity = parseInt($quantityDisplay.data('quantity'));
    const variantId = $(this).data('variant-id');
    const productId = $(this).data('product-id') || selectedProductId;

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
                productId: productId,
                variantId: variantId,
                quantity: quantity
            },
            success: function () {
                $("#alertMessageAddtoCart").removeClass("d-none").fadeIn();
                setTimeout(function () {
                    $("#alertMessageAddtoCart").fadeOut();
                    updateCartTotalQuantity();
                    updateCartTotalPrice();
                    updateTotalQuantity(productId);   //roma new
                    // Add reload here similar to the non-variant version
                    //location.reload();
                }, 2000);
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
            }
        });
    }
});

// Similarly update the minus-btn click handler
$(document).on('click', '.minus-btn', function () {
    const $row = $(this).closest('.variant-row');
    const $quantityDisplay = $row.find('.quantity-display');
    const $plusBtn = $row.find('.plus-btn');
    let quantity = parseInt($quantityDisplay.data('quantity'));
    const variantId = $(this).data('variant-id');
    const productId = $(this).data('product-id') || selectedProductId;

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
                productId: productId,
                variantId: variantId,
                quantity: quantity
            },
            success: function () {
                $("#alertMessageUpdateCart").removeClass("d-none").fadeIn();
                setTimeout(function () {
                    $("#alertMessageUpdateCart").fadeOut();
                    updateCartTotalQuantity();
                    updateCartTotalPrice();
                    updateTotalQuantity(productId);   //roma new
                    // Add reload here
                    //location.reload();
                }, 2000);
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
            }
        });
    }
});
    // Add this new code to handle modal close or Done button
    $('#variantModal').on('hidden.bs.modal', function () {
        // Reload the page when the modal is closed
  
        location.reload();
    });

    // Alternative: If you want a "Done" button in your modal
    $('#doneWithVariantsBtn').on('click', function () {
        $('#variantModal').modal('hide');
        // The hidden.bs.modal event will handle the reload
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

        if (!variantId || variantId === "" || variantId === "null" || variantId == null) {
            variantId = null;
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
                        $quantityDisplay.data('quantity', newQuantity);
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
                                updateTotalQuantity(productId);
                            },
                            error: function (xhr) {
                                $("#alertMessageFailToAdd").removeClass("d-none").fadeIn();
                                setTimeout(function () {
                                    $("#alertMessageFailToAdd").fadeOut();
                                }, 2000);
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
                    let newQuantity = currentQuantity + 1;
                    if (newQuantity <= stock) {
                        $quantityDisplay.text(newQuantity);
                        $quantityDisplay.data('quantity', newQuantity);
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
                                updateTotalQuantity(productId);
                            },
                            error: function () {
                                $quantityDisplay.text(currentQuantity);
                                $quantityDisplay.data('quantity', currentQuantity);
                                $("#alertMessageFailUpdate").removeClass("d-none").fadeIn();
                                setTimeout(function () {
                                    $("#alertMessageFailUpdate").fadeOut();
                                }, 2000);
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
        const variantId = $(this).data('variant-id') === "null" ? null : $(this).data('variant-id');

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
                    $quantityDisplay.data('quantity', newQuantity);
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
                            updateTotalQuantity(productId);
                        },
                        error: function () {
                            $quantityDisplay.text(currentQuantity);
                            $quantityDisplay.data('quantity', currentQuantity);
                            $("#alertMessageFailUpdate").removeClass("d-none").fadeIn();
                            setTimeout(function () {
                                $("#alertMessageFailUpdate").fadeOut();
                            }, 2000);
                        }
                    });
                }
            }
        });
    });
});




