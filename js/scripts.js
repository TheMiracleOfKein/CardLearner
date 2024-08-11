function flipCard(button) {
    const cardContainer = button.closest('.card-container');
    cardContainer.classList.add('flipped');
}

function flipFront(button) {
    const cardContainer = button.closest('.card-front');
    cardContainer.classList.add('flipped');
})

function flipBack(event, button) {
    event.stopPropagation();
    const cardContainer = button.closest('.card-container');
    cardContainer.classList.remove('flipped');
}

function selectAnswer(button, correctAnswer, selectedOption) {
    if (selectedOption === correctAnswer) {
        button.classList.add('correct');
        button.classList.remove('incorrect');
        disableOtherOptions(button);
    } else {
        button.classList.add('incorrect');
        button.classList.remove('correct');
    }
}

function disableOtherOptions(selectedButton) {
    let buttons = selectedButton.parentElement.getElementsByTagName('button');
    for (let i = 0; i < buttons.length; i++) {
        if (buttons[i] !== selectedButton && !buttons[i].classList.contains('flip-button')) {
            buttons[i].disabled = true;
        }
    }
}
