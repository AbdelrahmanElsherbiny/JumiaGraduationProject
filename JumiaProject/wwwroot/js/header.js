
$(document).ready(function () {
    var searchInput = $('#search-key');
    var searchResults = $('#searchResults');

    searchInput.on('input', function () {
        var query = $(this).val();
        if (query.length >= 1) {
            $.ajax({
                url: '/Product/Search',
                data: { query: query },
                type: 'GET',
                success: function (data) {
                    var dropdownContent = '';
                    if (data.length > 0) {
                        $.each(data, function (index, product) {
                            dropdownContent += '<a href="/Product/ProductDetails/' + product.productId + '" class="dropdown-item">' + product.name + '</a>';
                        });
                        searchResults.html(dropdownContent).fadeIn();
                    } else {
                        searchResults.html('<div class="dropdown-item">No results found</div>').fadeIn();
                    }
                },
                error: function () {
                    console.log('Error');
                }
            });
        } else {
            searchResults.fadeOut();
        }
    });
    searchInput.on('focus', function () {
        if ($(this).val().length >= 3) {
            searchResults.fadeIn();
        }
    });

    $(document).on('click', function (e) {
        if (!$(e.target).closest('#search-key').length && !$(e.target).closest('#searchResults').length) {
            searchResults.fadeOut();
        }
    });
});


$(document).ready(function () {
    var searchBrandResults = $('#searchBrandResults');
    var searchInput = $('#search-key');
    $('#search-btn').click(function () {
        var brand = searchInput.val().trim();
        if (brand.length > 0) {
            $.ajax({
                url: '/Product/IsBrandExist',
                data: { brand: brand },
                type: 'GET',
                success: function (data) {
                    console.log(data);
                    if (Number(data.result) > 0) {
                        window.location.href = `/Product/GetBrandProducts/${Number(data.result)}`
                    } else {
                        searchBrandResults.html('<div class="dropdown-item">No products found for this brand</div>').fadeIn();
                    }
                },
                error: function (data) {
                    console.log('Error');
                }
            });
        } else {
            searchBrandResults.fadeOut();
        }
    });
    searchInput.on('focus', function () {
        if ($(this).val().length >= 1) {
            searchBrandResults.fadeIn();
        }
    });
    searchInput.on('input', function () {
        if ($(this).val().length === 0) {
            searchBrandResults.fadeOut();
        }
    });
    $(document).on('click', function (e) {
        if (!$(e.target).closest('#search-key').length && !$(e.target).closest('#searchBrandResults').length) {
            searchBrandResults.fadeOut();
        }
    });
});


	function showList(){
		document.getElementById("dropdownList").classList.toggle("show");
	}

	window.onclick = function(event) {
	  if (!event.target.matches('.nav')) {
		var dropdowns = document.getElementsByClassName("list");
	var i;
	for (i = 0; i < dropdowns.length; i++) {
		  var openDropdown = dropdowns[i];
	if (openDropdown.classList.contains('show')) {
		openDropdown.classList.remove('show');
		  }
		}
	  }
	}

