document.addEventListener("DOMContentLoaded", function () {
    // Handle explanation toggle
    document.querySelectorAll(".toggle-explanation").forEach(function (button) {
        button.addEventListener("click", function () {
            var explanation = this.nextElementSibling;
            if (explanation.style.display === "none" || explanation.style.display === "") {
                explanation.style.display = "block";
                this.textContent = "Hide Explanation";
            } else {
                explanation.style.display = "none";
                this.textContent = "Show Explanation";
            }
        });
    });

    // Handle answer selection
    document.querySelectorAll(".answer-option").forEach(function (button) {
        button.addEventListener("click", function () {
            var questionId = this.getAttribute("data-question-id");
            var correctAnswer = this.getAttribute("data-correct-answer");
            var selectedAnswer = this.textContent;
            var options = document.querySelectorAll("#question-" + questionId + " .answer-option");

            if (selectedAnswer === correctAnswer) {
                options.forEach(function (opt) {
                    opt.disabled = true;
                    if (opt.textContent === correctAnswer) {
                        opt.style.backgroundColor = "green";
                        opt.style.color = "white";
                    }
                });
            } else {
                this.style.backgroundColor = "red";
                this.style.color = "white";
            }
        });
    });

    // Handle edit answer form toggle
    document.querySelectorAll(".edit-answer").forEach(function (button) {
        button.addEventListener("click", function () {
            var form = this.nextElementSibling;
            if (form.style.display === "none" || form.style.display === "") {
                form.style.display = "block";
            } else {
                form.style.display = "none";
            }

            // Toggle explanation form
            var explanationForm = this.closest(".question-block").querySelector(".edit-explanation-form");
            if (explanationForm.style.display === "none" || explanationForm.style.display === "") {
                explanationForm.style.display = "block";
            } else {
                explanationForm.style.display = "none";
            }
        });
    });

    // Handle save answer
    document.querySelectorAll(".save-answer").forEach(function (button) {
        button.addEventListener("click", async function () {
            var questionId = this.getAttribute("data-question-id");
            var select = document.getElementById("correct-answer-" + questionId);
            var newCorrectAnswer = select.value;

            // Find the question block
            var questionBlock = document.getElementById("question-" + questionId);
            var answerButtons = questionBlock.querySelectorAll(".answer-option");

            // Update the correct answer data attribute on each button
            answerButtons.forEach(function (btn) {
                btn.setAttribute("data-correct-answer", newCorrectAnswer);
                if (btn.textContent.trim() === newCorrectAnswer.trim()) {
                    btn.classList.add("correct");
                    btn.classList.remove("incorrect");
                } else {
                    btn.classList.remove("correct");
                    btn.classList.add("incorrect");
                }
            });

            // Hide the edit form
            this.parentElement.style.display = "none";

            // Optionally, send an AJAX request to update the correct answer on the server side
            updateCorrectAnswer(questionId, newCorrectAnswer);
        });
    });

    // Handle save explanation
    document.querySelectorAll(".save-explanation").forEach(function (button) {
        button.addEventListener("click", async function () {
            var questionId = this.getAttribute("data-question-id");
            var textarea = document.getElementById("explanation-" + questionId);
            var newExplanation = textarea.value;

            // Update the explanation text
            var explanationParagraph = this.parentElement.previousElementSibling;
            explanationParagraph.textContent = newExplanation;

            // Hide the edit form
            this.parentElement.style.display = "none";

            // Optionally, send an AJAX request to update the explanation on the server side
            updateExplanation(questionId, newExplanation);
        });
    });
});

// Example function to send AJAX request to update correct answer on the server side
function updateCorrectAnswer(questionId, newCorrectAnswer) {
    fetch(`/api/update-correct-answer`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ questionId, newCorrectAnswer })
    })
        .then(response => response.json())
        .then(data => {
            console.log('Success:', data);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}

// Example function to send AJAX request to update explanation on the server side
function updateExplanation(questionId, newExplanation) {
    fetch(`/api/question/update-explanation`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ questionId, newExplanation })
    })
        .then(response => response.json())
        .then(data => {
            console.log('Success:', data);
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}
