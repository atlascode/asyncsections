function checkDOMChange() {
	transform();
	if (document.readyState !== "complete") {
		setTimeout(checkDOMChange, 100);
	}
}

function transform() {
	var appends = [];
	var replaces = [];
	var removeClass = [];

	var allElements = document.getElementsByTagName('*');
	for (var i = 0; i < allElements.length; i++) {
		if (typeof allElements[i].getAttribute('transform-append') === 'string') {
			// Element exists with attribute. Add to array.
			appends.push(allElements[i]);
		} else if (typeof allElements[i].getAttribute('transform-replace') === 'string') {
			// Element exists with attribute. Add to array.
			replaces.push(allElements[i]);
		}
		if (typeof allElements[i].getAttribute('transform-removeClass') === 'string') {
			// Element exists with attribute. Add to array.
			removeClass.push(allElements[i]);
		}
	}

	for (var i = 0; i < appends.length; i++) {
		var element = appends[i];
		element.parentNode.removeChild(element);
		document.getElementById(element.getAttribute('transform-append')).appendChild(element);
		element.removeAttribute('transform-append');
	}

	for (var i = 0; i < removeClass.length; i++) {
		var element = removeClass[i];
		element.className = element.className.replace(new RegExp('(?:^|\s)' + element.getAttribute('transform-removeClass') + '(?!\S)', ''), '')
		element.removeAttribute('transform-removeClass');
	}

	for (var i = 0; i < replaces.length; i++) {
		var element = replaces[i];
		element.parentNode.removeChild(element);
		var placeholder = document.getElementById(element.getAttribute('transform-replace'));
		placeholder.parentNode.insertBefore(element, placeholder);
		placeholder.parentNode.removeChild(placeholder);
		element.removeAttribute('transform-replace');
	}
}

function endsWith(str, suffix) {
	return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

checkDOMChange();