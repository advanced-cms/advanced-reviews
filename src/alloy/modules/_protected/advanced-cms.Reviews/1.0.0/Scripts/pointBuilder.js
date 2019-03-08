define([
], function (
) {    
    function create(el) {
		var propertyEl = findPropertyElement(el);
		var result = {};
		if (propertyEl) {
			result.name = getPropertyName(propertyEl);
		}
		
		return result;
	}
	
	function findPropertyElement(el) {
		while(el) {
			var attribute = getPropertyName(el);
			if (attribute) {
				return el;
			}
			el = el.parentElement;
		}
		return null;
	}
	
	function getPropertyName(el) {
		return el.getAttribute("data-epi-property-name");
	}

    return {
        create: create
    };
});
