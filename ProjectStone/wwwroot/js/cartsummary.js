// BrainTree.
// Set to form instead of button (example: https://developer.paypal.com/braintree/docs/start/drop-in)
var form = document.querySelector('#payment-form');

braintree.dropin.create({
    authorization: clientToken, // 'CLIENT_TOKEN_FROM_SERVER'
    selector: '#dropin-container'
}, function(err, instance) {
    form.addEventListener('submit', function() {
        // Prevent the form from being posted automatically to the SummaryPost action method.
        event.preventDefault();

        // Add the nonce to the form, then submit.
        instance.requestPaymentMethod(function(err, payload) {
            // Submit payload.nonce to your server
            document.querySelector('#nonce').value = payload.nonce;
            // Now submit the form.
            form.submit();
        });
    });
});

// SweetAlert2.
// Post only if validateInput returns true. If false, do not post, display error.
function validateInput() {
    // Get input types. "." are translated to "_" for the element Ids of asp properties.
    const appName = document.getElementById("ApplicationUser_FullName").value;
    const appPhone = document.getElementById("ApplicationUser_PhoneNumber").value;
    const appEmail = document.getElementById("ApplicationUser_Email").value;
    const appAddress = document.getElementById("ApplicationUser_Address").value;
    const appCity = document.getElementById("ApplicationUser_City").value;
    const appState = document.getElementById("ApplicationUser_State").value;
    const appPostalCode = document.getElementById("ApplicationUser_PostalCode").value;

    // Check input type values.
    if (appName.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter Full Name to continue!'
        });
        // Return false so the form is not posted.
        return false;
    }
    if (appPhone.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter Phone Number to continue!'
        });
        return false;
    }
    if (appEmail.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter Email to continue!'
        });
        return false;
    }
    if (appAddress.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter Address to continue!'
        });
        return false;
    }
    if (appCity.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter City to continue!'
        });
        return false;
    }
    if (appState.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter State to continue!'
        });
        return false;
    }
    if (appPostalCode.toString() == '') {
        Swal.fire({
            icon: 'error',
            title: 'Well, this is awkward...',
            text: 'Please enter Postal Code to continue!'
        });
        return false;
    }

    // Return true if no error.
    return true;
}