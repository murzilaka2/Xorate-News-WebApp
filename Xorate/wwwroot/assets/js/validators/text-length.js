/*Подсчет символов для Input и TeaxtArea*/
function minLengthFunc(elementId, progressBarId, maxValue) {
    var inputLength = document.getElementById(elementId).value.length;
    var progressBar = document.getElementById(progressBarId);
    progressBar.setAttribute('aria-valuenow', inputLength);
    progressBar.style.width = 5 + (inputLength / maxValue) * 95 + "%";
    progressBar.textContent = inputLength + " / " + maxValue;
}

/*Подсчет символов для CkEditor5*/
function countCharactersInDiv(divClass) {
    const divs = document.getElementsByClassName(divClass);
    if (divs.length === 0) {
        return 0;
    }
    const div = divs[0];
    function countCharacters(node) {
        let count = 0;
        if (node.nodeType === Node.TEXT_NODE) {
            count += node.nodeValue.length;
        }
        if (node.hasChildNodes()) {
            node.childNodes.forEach(child => {
                count += countCharacters(child);
            });
        }
        return count;
    }
    return countCharacters(div);
}
function progressBarSet(progressBarId, maxValue, inputLength) {
    var progressBar = document.getElementById(progressBarId);
    progressBar.setAttribute('aria-valuenow', inputLength);
    progressBar.style.width = (inputLength / maxValue) * 100 + "%";
    progressBar.textContent = inputLength + " / " + maxValue;
}

function updateCharacterCount() {
    const divClass = "ck ck-editor__main";
    const characterCount = countCharactersInDiv(divClass);
    progressBarSet('description-bar', '∞', characterCount);
}

const divClass = "ck ck-editor__main";
const div = document.getElementsByClassName(divClass)[0];
div.addEventListener('keyup', updateCharacterCount);
updateCharacterCount();

window.onload = function (e) {
    minLengthFunc('title', 'title-bar', '80');
    minLengthFunc('alt', 'alt-bar', '40');
    minLengthFunc('keywords', 'keywords-bar', '70');
    minLengthFunc('seo-description', 'seo-description-bar', '160');
}