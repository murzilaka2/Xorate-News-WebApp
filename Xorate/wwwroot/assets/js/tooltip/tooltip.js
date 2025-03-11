function copy(that) {
    var inp = document.createElement('input');
    document.body.appendChild(inp);
    inp.value = that.textContent;
    inp.select();
    document.execCommand('copy');
    document.body.removeChild(inp);
    var tooltip = document.createElement('div');
    tooltip.textContent = 'Скопировано';
    tooltip.className = 'tooltip';
    var rect = that.getBoundingClientRect();
    var scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    var scrollLeft = window.pageXOffset || document.documentElement.scrollLeft;
    tooltip.style.top = rect.top + scrollTop + 'px';
    tooltip.style.left = rect.left + scrollLeft + 'px';
    document.body.appendChild(tooltip);

    setTimeout(function () {
        tooltip.style.opacity = 1;
    }, 10);

    setTimeout(function () {
        tooltip.style.opacity = 0;
        setTimeout(function () {
            tooltip.parentNode.removeChild(tooltip);
        }, 400);
    }, 2000);
}