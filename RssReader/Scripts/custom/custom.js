$(function () {    
    // register check all
	$("#checkAll").click(function () {
		$("input[name='feedIdsToDelete']").prop('checked', this.checked);
		setVisibilityOfDelete();
	});

	$("input[name='feedIdsToDelete']").click(function () {
		if ($("input[name='movieIdsToDelete']").length == $("input[name='feedIdsToDelete']:checked").length) {
			$('#checkAll').prop('checked', true);
		}
		else {
			$('#checkAll').prop('checked', false);
		}
		setVisibilityOfDelete();
	});


	$("#btnDeleteAll").click(function () {
		var count = $("input[name='feedIdsToDelete']:checked").length;
		if (count > 0) {
			return confirm(count + " record(s) will be deleted");
		}
	});
});

function setVisibilityOfDelete() {
	$('#btnDeleteAll').attr('disabled', 'disabled');

	var count = $("input[name='feedIdsToDelete']:checked").length;
	if (count > 0)
		$('input[type="submit"]').removeAttr('disabled');
}