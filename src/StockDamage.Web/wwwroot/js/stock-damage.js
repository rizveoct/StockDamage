(function ($) {
    const form = $('#stockDamageForm');
    const itemsTable = $('#itemsTable tbody');
    const totalAmountLabel = $('#totalAmount');
    const addRowBtn = $('#addRowBtn');
    const saveBtn = $('#saveBtn');

    const state = {
        items: []
    };

    function resetForm() {
        form.trigger('reset');
        $('#batchNo').val('NA');
        $('#drAccountHead').val('Stock Damage');
        $('#itemCode, #itemUnit, #itemStock').val('');
        $('#exchangeRate').val('');
    }

    function updateTotal() {
        const total = state.items.reduce((sum, item) => sum + (parseFloat(item.AmountIn) || 0), 0);
        totalAmountLabel.text(total.toFixed(2));
    }

    function renderTable() {
        itemsTable.empty();
        if (!state.items.length) {
            itemsTable.append('<tr class="no-data"><td colspan="17" class="text-center text-muted">No matching records found</td></tr>');
            updateTotal();
            return;
        }

        state.items.forEach((item, index) => {
            const row = `<tr data-index="${index}">
                <td>${index + 1}</td>
                <td>${item.WarehouseName}</td>
                <td>${item.SubItemName}</td>
                <td>${item.SubItemCode}</td>
                <td>${item.Unit}</td>
                <td>${item.Stock}</td>
                <td>${item.BatchNo}</td>
                <td>${item.CurrencyName}</td>
                <td>${item.Quantity}</td>
                <td>${item.Rate}</td>
                <td>${parseFloat(item.AmountIn).toFixed(2)}</td>
                <td>${item.CurrencyRate}</td>
                <td>${item.DrAccountHead}</td>
                <td>${item.EmployeeName}</td>
                <td>${item.Comments || ''}</td>
                <td class="text-center"><button type="button" class="btn btn-outline-secondary btn-sm edit-row"><i class="bi bi-pencil-square"></i></button></td>
                <td class="text-center"><button type="button" class="btn btn-outline-danger btn-sm delete-row"><i class="bi bi-trash"></i></button></td>
            </tr>`;
            itemsTable.append(row);
        });

        updateTotal();
    }

    function normaliseDecimal(value) {
        const parsed = parseFloat(value);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function collectFormData() {
        const formData = Object.fromEntries(new FormData(form[0]).entries());
        const warehouseOption = $('#warehouseSelect option:selected');
        const currencyOption = $('#currencySelect option:selected');
        const itemOption = $('#itemSelect option:selected');
        const employeeOption = $('#employeeSelect option:selected');

        formData.GodownNo = parseInt(formData.GodownNo, 10) || 0;
        formData.WarehouseName = warehouseOption.data('name') || '';
        formData.CurrencyRate = currencyOption.data('rate') || 0;
        formData.CurrencyName = currencyOption.val() || '';
        formData.SubItemName = itemOption.data('name') || '';
        formData.Unit = $('#itemUnit').val();
        formData.Stock = normaliseDecimal($('#itemStock').val());
        formData.SubItemCode = $('#itemCode').val();
        formData.EmployeeName = employeeOption.data('name') || '';
        formData.EmployeeId = parseInt(employeeOption.val(), 10) || 0;
        formData.BatchNo = $('#batchNo').val();
        formData.DrAccountHead = $('#drAccountHead').val();
        formData.CurrencyRate = normaliseDecimal(formData.CurrencyRate);
        formData.Quantity = normaliseDecimal(formData.Quantity);
        formData.Rate = normaliseDecimal(formData.Rate);
        formData.AmountIn = normaliseDecimal(formData.AmountIn);

        return formData;
    }

    function populateForm(item) {
        $('[name="Date"]').val(item.Date.split('T')[0]);
        $('[name="VoucherNo"]').val(item.VoucherNo);
        $('#warehouseSelect').val(item.GodownNo);
        $('#currencySelect').val(item.CurrencyName);
        $('#itemSelect').val(item.SubItemCode);
        $('#itemCode').val(item.SubItemCode);
        $('#itemUnit').val(item.Unit);
        $('#itemStock').val(item.Stock);
        $('#batchNo').val(item.BatchNo);
        $('#exchangeRate').val(item.CurrencyRate);
        $('#quantity').val(item.Quantity);
        $('#rate').val(item.Rate);
        $('#amountIn').val(item.AmountIn);
        $('#drAccountHead').val(item.DrAccountHead);
        $('#employeeSelect').val(item.EmployeeId);
        $('#comments').val(item.Comments);
    }

    function addOrUpdateItem(data, index = null) {
        if (index !== null) {
            state.items[index] = data;
        } else {
            state.items.push(data);
        }
        renderTable();
        resetForm();
    }

    addRowBtn.on('click', function () {
        const data = collectFormData();
        if (!data.GodownNo || !data.SubItemCode || !data.EmployeeId || !data.CurrencyName) {
            alert('Please fill in all required fields before adding.');
            return;
        }
        addOrUpdateItem(data);
    });

    itemsTable.on('click', '.delete-row', function () {
        const rowIndex = $(this).closest('tr').data('index');
        state.items.splice(rowIndex, 1);
        renderTable();
    });

    itemsTable.on('click', '.edit-row', function () {
        const rowIndex = $(this).closest('tr').data('index');
        const item = state.items[rowIndex];
        populateForm(item);
        state.items.splice(rowIndex, 1);
        renderTable();
    });

    $('#itemSelect').on('change', function () {
        const selectedOption = $(this).find('option:selected');
        const code = selectedOption.val();
        if (!code) {
            $('#itemCode, #itemUnit, #itemStock').val('');
            return;
        }

        $('#itemCode').val(code);
        $('#itemUnit').val(selectedOption.data('unit'));
        $('#itemStock').val('Loading...');

        fetch(`?handler=SubItem&code=${code}`)
            .then(response => response.ok ? response.json() : Promise.reject('Unable to fetch item'))
            .then(data => {
                $('#itemCode').val(data.subItemCode);
                $('#itemUnit').val(data.unit);
                $('#itemStock').val(data.stock);
            })
            .catch(() => {
                $('#itemStock').val('0');
            });
    });

    $('#currencySelect').on('change', function () {
        const rate = $(this).find('option:selected').data('rate');
        $('#exchangeRate').val(rate || '');
    });

    $('#quantity, #rate').on('input', function () {
        const quantity = parseFloat($('#quantity').val());
        const rate = parseFloat($('#rate').val());
        if (!isNaN(quantity) && !isNaN(rate)) {
            $('#amountIn').val((quantity * rate).toFixed(2));
        }
    });

    $('#amountIn').on('change', function () {
        const quantity = parseFloat($('#quantity').val());
        const amount = parseFloat($('#amountIn').val());
        if (!isNaN(quantity) && quantity !== 0 && !isNaN(amount)) {
            $('#rate').val((amount / quantity).toFixed(2));
        }
    });

    saveBtn.on('click', function () {
        if (!state.items.length) {
            alert('Please add at least one item before saving.');
            return;
        }

        const payload = {
            Date: $('[name="Date"]').val(),
            VoucherNo: $('[name="VoucherNo"]').val(),
            DrAccountHead: $('#drAccountHead').val(),
            Items: state.items
        };

        fetch('', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            body: JSON.stringify(payload)
        })
            .then(response => response.ok ? response.json() : Promise.reject('Save failed'))
            .then(() => {
                alert('Stock damage saved successfully.');
                state.items = [];
                renderTable();
                resetForm();
            })
            .catch(() => alert('Unable to save stock damage entries. Please try again.'));
    });

    renderTable();
})(jQuery);
