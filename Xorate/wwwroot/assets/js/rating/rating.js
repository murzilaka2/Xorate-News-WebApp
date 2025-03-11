document.addEventListener("DOMContentLoaded", function () {
    var addButton = document.getElementById('addInputButton');
    var removeButton = document.getElementById('removeInputButton');
    var container = document.getElementById('inputContainer');
    var count = document.querySelectorAll('input[name="rating"]').length;


    addButton.addEventListener('click', function () {
        if (count < 10) {
            count++;
            var input = document.createElement('input');
            input.type = 'text';
            input.name = 'rating';
            input.placeholder = 'Позиция в рейтинге #️⃣' + count + ':';
            input.className = 'input-container';
            container.appendChild(input);

            if (count === 10) {
                addButton.style.display = 'none';
            }
        }
        if (count == 0) {
            removeButton.style.display = 'none';
        }
        else {
            removeButton.style.display = 'inline-block';
        }
    });

    removeButton.addEventListener('click', function () {
        if (count > 0) {
            var lastInput = container.lastElementChild;
            container.removeChild(lastInput);
            count--;

            addButton.style.display = 'inline-block';
            addButton.disabled = false;
            addButton.textContent = 'Добавить позицию';
        }
        if (count == 0) {
            removeButton.style.display = 'none';
        }
        else {
            removeButton.style.display = 'inline-block';
        }
    });

});