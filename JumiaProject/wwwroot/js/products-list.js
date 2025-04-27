document.addEventListener('DOMContentLoaded', function () {
    // Delete product confirmation setup
    document.querySelectorAll('.delete-product').forEach(button => {
        button.addEventListener('click', function () {
            const productId = this.getAttribute('data-id');
            const productName = this.getAttribute('data-name');

            document.getElementById('deleteProductName').textContent = productName;
            document.getElementById('confirmDelete').href = '/Seller/DeleteProduct/' + productId;
        });
    });

    // Search functionality
    const searchName = document.getElementById('searchName');
    const searchSKU = document.getElementById('searchSKU');
    const productsTable = document.getElementById('productsTable');

    function filterTable() {
        const nameFilter = searchName.value.toLowerCase();
        const skuFilter = searchSKU.value.toLowerCase();

        const rows = productsTable.querySelectorAll('tbody tr');

        rows.forEach(row => {
            const nameCell = row.cells[1].textContent.toLowerCase();
            const skuCell = row.cells[2].textContent.toLowerCase();

            const nameMatch = nameCell.includes(nameFilter);
            const skuMatch = skuCell.includes(skuFilter);

            if (nameMatch && skuMatch) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });

        updatePaginationInfo();
    }

    searchName.addEventListener('input', filterTable);
    searchSKU.addEventListener('input', filterTable);

    // Pagination functionality
    const itemsPerPageSelect = document.getElementById('itemsPerPage');
    const prevPageBtn = document.getElementById('prevPage');
    const nextPageBtn = document.getElementById('nextPage');
    let currentPage = 1;

    function updatePaginationInfo() {
        const itemsPerPage = parseInt(itemsPerPageSelect.value);
        const rows = Array.from(productsTable.querySelectorAll('tbody tr')).filter(row =>
            row.style.display !== 'none'
        );

        const totalItems = rows.length;
        const totalPages = Math.ceil(totalItems / itemsPerPage);

        // Update displayed rows
        rows.forEach((row, index) => {
            if (index >= (currentPage - 1) * itemsPerPage && index < currentPage * itemsPerPage) {
                row.classList.add('visible-row');
                row.style.display = '';
            } else if (row.style.display !== 'none') {
                row.classList.remove('visible-row');
                row.style.display = 'none';
            }
        });

        // Update pagination text
        const start = Math.min(totalItems, (currentPage - 1) * itemsPerPage + 1);
        const end = Math.min(totalItems, currentPage * itemsPerPage);

        document.getElementById('currentStart').textContent = totalItems > 0 ? start : 0;
        document.getElementById('currentEnd').textContent = end;
        document.getElementById('totalItems').textContent = totalItems;

        // Update pagination buttons
        prevPageBtn.parentElement.classList.toggle('disabled', currentPage === 1);
        nextPageBtn.parentElement.classList.toggle('disabled', currentPage >= totalPages);

        // Update page numbers
        const paginationList = document.querySelector('.pagination');
        // Clear existing page numbers
        Array.from(paginationList.querySelectorAll('li:not(:first-child):not(:last-child)')).forEach(li => {
            li.remove();
        });

        // Add page numbers
        const maxPages = Math.min(totalPages, 5);
        let startPage = Math.max(1, currentPage - 2);
        if (currentPage > totalPages - 2) {
            startPage = Math.max(1, totalPages - 4);
        }

        for (let i = startPage; i < startPage + maxPages && i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = `page-item ${i === currentPage ? 'active' : ''}`;

            const a = document.createElement('a');
            a.className = 'page-link';
            a.href = '#';
            a.textContent = i;

            a.addEventListener('click', (e) => {
                e.preventDefault();
                currentPage = i;
                updatePaginationInfo();
            });

            li.appendChild(a);
            paginationList.insertBefore(li, nextPageBtn.parentElement);
        }
    }

    itemsPerPageSelect.addEventListener('change', () => {
        currentPage = 1;
        updatePaginationInfo();
    });

    prevPageBtn.addEventListener('click', (e) => {
        e.preventDefault();
        if (currentPage > 1) {
            currentPage--;
            updatePaginationInfo();
        }
    });

    nextPageBtn.addEventListener('click', (e) => {
        e.preventDefault();
        const rows = Array.from(productsTable.querySelectorAll('tbody tr')).filter(row =>
            row.style.display !== 'none'
        );
        const totalItems = rows.length;
        const itemsPerPage = parseInt(itemsPerPageSelect.value);
        const totalPages = Math.ceil(totalItems / itemsPerPage);

        if (currentPage < totalPages) {
            currentPage++;
            updatePaginationInfo();
        }
    });

    // Filter functionality
    const applyFiltersBtn = document.getElementById('applyFilters');
    const resetFiltersBtn = document.getElementById('resetFilters');

    function applyFilters() {
        const minPrice = parseFloat(document.getElementById('minPrice').value) || 0;
        const maxPrice = parseFloat(document.getElementById('maxPrice').value) || Infinity;
        const brand = document.getElementById('brandFilter').value;
        const discount = document.getElementById('discountFilter').value;
        const minStock = parseInt(document.getElementById('minStock').value) || 0;
        const maxStock = parseInt(document.getElementById('maxStock').value) || Infinity;
        const minSold = parseInt(document.getElementById('minSold').value) || 0;
        const maxSold = parseInt(document.getElementById('maxSold').value) || Infinity;
        const visibility = document.getElementById('visibilityFilter').value;

        const rows = productsTable.querySelectorAll('tbody tr');

        rows.forEach(row => {
            // Skip rows that are already hidden by search
            if (row.style.display === 'none' &&
                (searchName.value !== '' || searchSKU.value !== '')) {
                return;
            }

            const price = parseFloat(row.cells[4].textContent.replace('$', ''));
            const brandCell = row.cells[3].textContent; // Using Category as Brand for demo
            const discountBadge = row.cells[5].querySelector('.badge');
            const hasDiscount = discountBadge !== null;
            const stock = parseInt(row.cells[6].textContent);
            const sold = parseInt(row.cells[7].textContent);
            const isVisible = row.cells[8].querySelector('i').classList.contains('text-success');

            let showRow = true;

            // Price filter
            if (price < minPrice || price > maxPrice) {
                showRow = false;
            }

            // Brand filter
            if (brand && brandCell !== brand) {
                showRow = false;
            }

            // Discount filter
            if (discount === 'yes' && !hasDiscount) {
                showRow = false;
            } else if (discount === 'no' && hasDiscount) {
                showRow = false;
            }

            // Stock filter
            if (stock < minStock || stock > maxStock) {
                showRow = false;
            }

            // Sold filter
            if (sold < minSold || sold > maxSold) {
                showRow = false;
            }

            // Visibility filter
            if (visibility === 'yes' && !isVisible) {
                showRow = false;
            } else if (visibility === 'no' && isVisible) {
                showRow = false;
            }

            row.style.display = showRow ? '' : 'none';
        });

        currentPage = 1;
        updatePaginationInfo();
    }

    function resetFilters() {
        document.getElementById('minPrice').value = '';
        document.getElementById('maxPrice').value = '';
        document.getElementById('brandFilter').value = '';
        document.getElementById('discountFilter').value = '';
        document.getElementById('minStock').value = '';
        document.getElementById('maxStock').value = '';
        document.getElementById('minSold').value = '';
        document.getElementById('maxSold').value = '';
        document.getElementById('visibilityFilter').value = '';

        // Reset sliders
        document.getElementById('priceRange').value = 500;
        document.getElementById('stockRange').value = 500;
        document.getElementById('soldRange').value = 500;

        // Show all rows that aren't filtered by search
        const rows = productsTable.querySelectorAll('tbody tr');
        rows.forEach(row => {
            if (searchName.value === '' && searchSKU.value === '') {
                row.style.display = '';
            } else {
                // Re-run search filter
                const nameCell = row.cells[1].textContent.toLowerCase();
                const skuCell = row.cells[2].textContent.toLowerCase();

                const nameMatch = nameCell.includes(searchName.value.toLowerCase());
                const skuMatch = skuCell.includes(searchSKU.value.toLowerCase());

                row.style.display = (nameMatch && skuMatch) ? '' : 'none';
            }
        });

        currentPage = 1;
        updatePaginationInfo();
    }

    applyFiltersBtn.addEventListener('click', applyFilters);
    resetFiltersBtn.addEventListener('click', resetFilters);

    // Slider functionality for filter modal
    const priceRange = document.getElementById('priceRange');
    const minPrice = document.getElementById('minPrice');
    const maxPrice = document.getElementById('maxPrice');

    priceRange.addEventListener('input', function () {
        const value = this.value;
        minPrice.value = Math.floor(value / 2);
        maxPrice.value = value;
    });

    const stockRange = document.getElementById('stockRange');
    const minStock = document.getElementById('minStock');
    const maxStock = document.getElementById('maxStock');

    stockRange.addEventListener('input', function () {
        const value = this.value;
        minStock.value = Math.floor(value / 2);
        maxStock.value = value;
    });

    const soldRange = document.getElementById('soldRange');
    const minSold = document.getElementById('minSold');
    const maxSold = document.getElementById('maxSold');

    soldRange.addEventListener('input', function () {
        const value = this.value;
        minSold.value = Math.floor(value / 2);
        maxSold.value = value;
    });

    // Initialize pagination
    updatePaginationInfo();
});