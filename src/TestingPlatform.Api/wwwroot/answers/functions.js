async function GetListFunction() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/Answers/List?resultId=" + urlParams.get('resultId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const questions = await response.json();
        let rows = document.getElementById("answersList");
        questions.forEach(answer => {
            rows.append(row(answer));
        });
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

function row(answer) {
    const tr = document.createElement("tr");

    const userAnswer = document.createElement("td");
    userAnswer.append(answer.userAnswer);
    tr.append(userAnswer);

    const rightAnswer = document.createElement("td");
    rightAnswer.append(answer.rightAnswer);
    tr.append(rightAnswer);

    const result = document.createElement("td");
    const boolResult = answer.userAnswer != undefined
        && answer.rightAnswer != undefined
        && answer.userAnswer.trim().toUpperCase() === answer.rightAnswer.trim().toUpperCase();
    result.append(boolResult ? "Верно" : "Не верно");
    tr.append(result);

    return tr;
}

GetListFunction()