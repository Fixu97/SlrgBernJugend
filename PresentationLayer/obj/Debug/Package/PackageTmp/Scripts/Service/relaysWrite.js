
	function getSelectedDisciplines() {
		var selectBox = $("#" + getSelectedSelectId());
		var options = selectBox.find("option");

		var disciplines = [];
		$(options).each(function (i, option) {
			disciplines.push($(option).val());
		});

		return disciplines;
	}

	function addToSelection() {
		var selectedOptions = getSelection(getAvailableSelectId());

		$(selectedOptions).each(function (i, option) {
			$(option).clone().appendTo($("#" + getSelectedSelectId()));
		});
	}

	function removeFromSelection() {
		var selectedOptions = getSelection(getSelectedSelectId());

		$(selectedOptions).each(function (i, option) {
			option.remove();
		});
	}

	function getSelection(id) {
		return $("#" + id).find(":selected");
	}

	function getAvailableSelectId() {
		return "multiselect-available";
	}

	function getSelectedSelectId() {
		return "multiselect-selected";
	}