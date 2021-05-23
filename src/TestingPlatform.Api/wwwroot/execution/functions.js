async function SendFunction() {
    const questions = JSON.parse(localStorage.getItem('questions'));
    const answers = [];
    Array.from(questions).forEach(question => {
        console.log(question.id);
        const val = document.getElementById(question.id).value;
        console.log(val);
        answers.push({ QuestionId: question.id, UserAnswer: val })
    })

    const response = await fetch("../api/v1/Answers/SendList", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(answers)
    });
    if (response.ok === true) {
        const resultId = await response.json();
        location.href = '../answers?resultId=' + resultId;
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

async function GetListFunction() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/Questions/List?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const questions = await response.json();
        let rows = document.getElementById("questionsList");
        questions.forEach(question => {
            rows.append(row(question));
        });
        localStorage.setItem('questions', JSON.stringify(questions));
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

async function GetTestInfoFunction() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/Tests?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const test = await response.json();
        document.getElementById("questionName").append(test.name);
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

function row(question) {
    const mainDiv = document.createElement("div");

    const pNum = document.createElement("p");
    pNum.append("#" + question.id);
    mainDiv.append(pNum);

    const pQuestion = document.createElement("p");
    pQuestion.append("question: " + question.question);
    mainDiv.append(pQuestion);

    const labelAnswer = document.createElement("label");
    labelAnswer.setAttribute("htmlFor", question.id);
    labelAnswer.append("answer: ");
    mainDiv.append(labelAnswer);

    const inputAnswer = document.createElement("input");
    inputAnswer.setAttribute("type", "text");
    inputAnswer.setAttribute("id", question.id);
    inputAnswer.setAttribute("name", question.id);
    mainDiv.append(inputAnswer);

    return mainDiv;
}

GetListFunction()
GetTestInfoFunction()