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
        document.getElementById("questionName").value = test.name;
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
    
    const labelQuestion = document.createElement("label");
    labelQuestion.setAttribute("htmlFor", "q" + question.id);
    labelQuestion.append("question: ");
    mainDiv.append(labelQuestion);
    
    const inputQuestion = document.createElement("input");
    inputQuestion.setAttribute("type", "text");
    inputQuestion.setAttribute("id", "q" + question.id);
    inputQuestion.setAttribute("name", "q" + question.id);
    inputQuestion.setAttribute("value", question.question);
    mainDiv.append(inputQuestion);
    
    const br1 = document.createElement("br");
    mainDiv.append(br1);

    const labelAnswer = document.createElement("label");
    labelAnswer.setAttribute("htmlFor", "a" + question.id);
    labelAnswer.append("answer: ");
    mainDiv.append(labelAnswer);

    const inputAnswer = document.createElement("input");
    inputAnswer.setAttribute("type", "text");
    inputAnswer.setAttribute("id", "a" + question.id);
    inputAnswer.setAttribute("name", "a" + question.id);
    inputAnswer.setAttribute("value", question.answer);
    mainDiv.append(inputAnswer);
    
    const br2 = document.createElement("br");
    mainDiv.append(br2);
    
    const button = document.createElement("button");
    button.setAttribute("onClick", "SaveFunction(\'"+question.id+"\')");
    button.append("Save");
    mainDiv.append(button);

    const delButton = document.createElement("button");
    delButton.setAttribute("onClick", "RemoveFunction(\'"+question.id+"\')");
    delButton.append("Remove");
    mainDiv.append(delButton);
    
    return mainDiv;
}

async function SaveFunction(num){
    const response = await fetch("../api/v1/Questions", {
        method: "PATCH",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            Id: num,
            Question: document.getElementById("q" + num).value,
            Answer: document.getElementById("a" + num).value
        })
    });
    if (response.ok === true) {
        location.reload();
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

async function AddFunction() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/Questions/AddEmpty?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        location.reload();
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

async function RemoveFunction(id) {
    const response = await fetch("../api/v1/Questions?questionId=" + id, {
        method: "DELETE"
    });
    if (response.ok === true) {
        location.reload();
    }
    if (response.status === 401) {
        location.href = './auth';
    }
}

async function UpdateTestInfoFunction() {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('testId');

    const response = await fetch("../api/v1/Tests?testId=" + id, {
        method: "PATCH",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            Id: id,
            Name: document.getElementById("questionName").value
        })
    });
    if (response.ok === true) {
        location.reload();
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

GetListFunction()
GetTestInfoFunction()