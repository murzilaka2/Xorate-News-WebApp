function minLengthFunc(elementId, progressBarId, maxValue) {
    var inputLength = document.getElementById(elementId).value.length;
    var progressBar = document.getElementById(progressBarId);
    progressBar.setAttribute('aria-valuenow', inputLength);
    progressBar.style.width = 7 + (inputLength / maxValue) * 93 + "%";
    progressBar.textContent = inputLength + " / " + maxValue;
}
function setFileds() {
    minLengthFunc('comment', 'comment-bar', '1000');
}