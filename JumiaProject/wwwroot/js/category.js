
    $(document).ready(function () {
        // Function to collect all filter values and call AJAX
        function applyFilters() {
            var selectedSizes = $("input[name='sizeIds']:checked").map(function () {
                return $(this).val();
            }).get();

            var selectedBrands = $("input[name='brandIds']:checked").map(function () {
                return $(this).val();
            }).get();

            var selectedDiscounts = $("input[name='discounts']:checked").map(function () {
                return $(this).val();
            }).get();

            var minPrice = $("#minPriceInput").val();
            var maxPrice = $("#maxPriceInput").val();
            var categoryId = $("#CategoryId").val(); // لازم يكون فيه hidden input فيه الكاتيجوري

            $.ajax({
                url: '/Category/FilterProducts', // غيري دي لو اسم الأكشن مختلف
                type: 'GET',
                data: {
                    id: categoryId,
                    sizeIds: selectedSizes,
                    brandIds: selectedBrands,
                    discountList: selectedDiscounts,
                    minPrice: minPrice,
                    maxPrice: maxPrice
                },
                success: function (response) {
                    // Replace the products div with the new content
                    $('.cartitemcat-container').html(response);
                },
                error: function () {
                    alert("حصلت مشكلة أثناء تحميل المنتجات");
                }
            });
        }

        // لما أي فلتر يتغير
        $("input[name='sizeIds'], input[name='brandIds'], input[name='discounts']").on('change', function () {
        applyFilters();
        });

    // لما المستخدم يضغط Apply على الفلتر بتاع السعر
    $('#applyPriceBtn').on('click', function () {
        applyFilters();
        });
    });

