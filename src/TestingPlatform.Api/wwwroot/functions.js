async function GetTests() {
    const response = await fetch("../api/v1/Tests/List", {
        method: "GET"
    });
    if (response.ok === true) {
        const tests = await response.json();
        let rows = document.querySelector("tbody");
        tests.forEach(test => {
            rows.append(row(test));
        });
    }
    if (response.status === 401) {
        location.href = './auth';
    }
}

function row(test) {
    const tr = document.createElement("tr");

    const name = document.createElement("td");
    const aHref = document.createElement("a");
    aHref.append(test.name);
    aHref.setAttribute("href", "./execution?testId=" + test.id)
    name.append(aHref);
    tr.append(name);
    
    const tRef = document.createElement("td");
    const tHref = document.createElement("a");
    tHref.append("ссылка");
    tHref.setAttribute("href", "./edit?testId=" + test.id)
    tRef.append(tHref);
    tr.append(tRef);

    const rRef = document.createElement("td");
    const rHref = document.createElement("a");
    rHref.append("ссылка");
    rHref.setAttribute("href", "./results?testId=" + test.id)
    rRef.append(rHref);
    tr.append(rRef);
    
    const remove = document.createElement("td");
    const button = document.createElement("button");
    button.setAttribute("onClick", "RemoveFunction(\'"+test.id+"\')");
    button.append("удалить");
    remove.append(button);
    tr.append(remove);
    
    return tr;
}

async function AddFunction() {
    const response = await fetch("../api/v1/Tests/AddEmpty", {
        method: "GET"
    });
    if (response.ok === true) {
        location.reload();
    }
    if (response.status === 401) {
        location.href = './auth';
    }
}

async function RemoveFunction(id) {
    const response = await fetch("../api/v1/Tests?testId=" + id, {
        method: "DELETE"
    });
    if (response.ok === true) {
        location.reload();
    }
    if (response.status === 401) {
        location.href = './auth';
    }
}

GetTests();