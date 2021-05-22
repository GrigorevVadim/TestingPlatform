async function SendFunction() {
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
        console.log(test.name);
        console.log(document.getElementById("questionName"));
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