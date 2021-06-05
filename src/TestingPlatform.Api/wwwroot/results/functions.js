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

GetListFunction()