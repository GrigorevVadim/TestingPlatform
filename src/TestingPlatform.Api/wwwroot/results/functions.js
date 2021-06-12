async function GetListFunction() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/statistics/GetResultList?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const questions = await response.json();
        let rows = document.getElementById("resultsList");
        questions.forEach(result => {
            rows.append(row(result));
        });
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

function row(result) {
    const tr = document.createElement("tr");

    const id = document.createElement("td");
    const aHref = document.createElement("a");
    aHref.append(result.id);
    aHref.setAttribute("href", "../answers?resultId=" + result.id)
    id.append(aHref);
    tr.append(id);

    const dateTime = document.createElement("td");
    dateTime.append(result.dateTime);
    tr.append(dateTime);

    const score = document.createElement("td");
    score.append(result.score);
    tr.append(score);

    const userLogin = document.createElement("td");
    userLogin.append(result.userLogin);
    tr.append(userLogin);

    return tr;
}

async function GetScorePerQuestions() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/statistics/GetScorePerQuestions?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const questions = await response.json();
        let rows = document.getElementById("scorePerQuestions");
        questions.forEach(result => {
            rows.append(rowScorePerQuestions(result));
        });
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

function rowScorePerQuestions(result) {
    const tr = document.createElement("tr");

    const question = document.createElement("td");
    question.append(result.question);
    tr.append(question);

    const score = document.createElement("td");
    score.append(result.score);
    tr.append(score);

    return tr;
}

async function GetScoreDistribution() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/statistics/GetScoreDistribution?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const questions = await response.json();
        let rows = document.getElementById("scoreDistribution");
        let num = 0;
        questions.forEach(result => {
            rows.append(rowScoreDistribution(result, num));
            num += 25;
        });
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

function rowScoreDistribution(result, num) {
    const tr = document.createElement("tr");

    const maxNum = num + 25;
    const score = document.createElement("td");
    score.append(num.toString() + "-" + maxNum.toString());
    tr.append(score);

    const percent = document.createElement("td");
    percent.append(result);
    tr.append(percent);

    return tr;
}

async function GetAverageScoreFunction() {
    const urlParams = new URLSearchParams(window.location.search);

    const response = await fetch("../api/v1/Statistics/GetAverageScore?testId=" + urlParams.get('testId'), {
        method: "GET"
    });
    if (response.ok === true) {
        const averageScore = await response.json();
        document.getElementById("averageScore").append("average: " + averageScore);
    }
    if (response.status === 401) {
        location.href = '../auth';
    }
}

GetListFunction()
GetAverageScoreFunction()
GetScorePerQuestions()
GetScoreDistribution()