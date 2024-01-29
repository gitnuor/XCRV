function showAccountList() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowListOfAccount",
            data: { customerId: customerId },
            success: function (data) {
                $("#list-of-accounts").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}


function showAccountDetails() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowAccountDetails",
            data: { customerId: customerId },
            success: function (data) {
                $("#account-details").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showNomineeDetails() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowNominees",
            data: { customerId: customerId },
            success: function (data) {
                $("#nominee-details").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showRelatedPartyDetails() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowRelatedParty",
            data: { customerId: customerId },
            success: function (data) {
                $("#related-party").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showChequeDetailsDetails() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowChequeDetails",
            data: { customerId: customerId },
            success: function (data) {
                $("#cheque-details").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showEftDetails() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowEftDetails",
            data: { customerId: customerId },
            success: function (data) {
                $("#eft-details").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showEftInfo( acccountNumber, divName, name) {

    if (acccountNumber) {
        $.ajax({
            url: "CustomerDashboard/ShowEftDetails",
            data: { acno: acccountNumber },
            success: function (data) {
                $("#"+divName).html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}


function showCollectedDocumentsInfo() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowCollectedDocuments",
            data: { customerId: customerId },
            success: function (data) {
                $("#collected-document").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showDebitCardDetailsInfo() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowDebitCardInfo",
            data: { customerId: customerId },
            success: function (data) {
                $("#debit-card-details").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}

function showDebitCardDetails(seachType, cardNumber, divName) {
    if (cardNumber) {
        $.ajax({
            url: "DebitCard/ShowDebitCardDetails",
            data: { seachType: seachType, searchString: cardNumber, maskedCardNo: '' },
            success: function (data) {
                $("#" + divName).html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Card Number is required!!!!');
    }
}

function showMemoPadInfo() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/GetCustomerMemosByCif",
            data: { cif: customerId },
            success: function (data) {
                $("#memo-pad").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}


function showIbDetailsInfo() {
    var customerId = $('#hdn_cust_id').val();

    if (customerId) {
        $.ajax({
            url: "CustomerDashboard/ShowIBInfo",
            data: { customerId: customerId },
            success: function (data) {
                $("#ib-details").html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Customer Number is required!!!!');
    }
}



function showIbDetailsDetails(acno, divName) {
    if (acno) {
        $.ajax({
            url: "CustomerDashboard/ShowIBDetailsInfo",
            data: { acno: acno, },
            success: function (data) {
                $("#" + divName).html(data);
            }
        });
    }
    else {
        toastr.error('Sorry!!! Account Number is required!!!!');
    }
}
