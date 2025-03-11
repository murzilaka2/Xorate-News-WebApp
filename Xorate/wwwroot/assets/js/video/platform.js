document.addEventListener('DOMContentLoaded', function () {
    var figures = document.querySelectorAll('figure.media');
    figures.forEach(function (figure) {
        var oembed = figure.querySelector('oembed');

        if (oembed) {
            var url = oembed.getAttribute('url');
            var iframe = document.createElement('iframe');
            iframe.setAttribute('width', '100%'); 
            iframe.setAttribute('height', '400'); 
            iframe.setAttribute('src', url); 
            iframe.setAttribute('frameborder', '0'); 
            iframe.setAttribute('allowfullscreen', ''); 
            figure.parentNode.replaceChild(iframe, figure);
        }
    });
});