$(document).ready(function() {
	$("#chooseButton").click(function() {
		//alert("Hello from JavaScript!");
		window.external.Notify("chooseAddress");
	});
});

function addressChooserCompleted(address) {
    $("#address").val(address);
}
