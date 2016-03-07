/***************************************************************/
/*
/*    Subscribe to EventSource /events from the server and 
/*    reload spa.min.js when a message is received
/*
/***************************************************************/

function reload(src) {
    $('script[src="' + src + '"]').remove();
    $('<script>').attr('src', src).append('body');
}

var es = new EventSource("/events"); 

$(function() {
	es.onmessage = function(e) {
		$("#main").empty();
		reload('Content/spa.min.js');
//		location.reload(true);
	}
})