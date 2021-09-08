$(document).ready(function() {
    const shippingDate = document.getElementById("shippingDate");

    // Hide date if value is 1/1/0001. ToDo: find better way to do this. Perhaps do validation on backend.
    if (shippingDate.value == '1/1/0001') {
        shippingDate.value = "";
    }
});