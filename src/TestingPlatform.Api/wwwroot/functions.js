async function GetStatus() {
    const response = await fetch("../api/v1/Status", {
        method: "GET"
    });
    if (response.ok === true) {
        document.getElementById("result").innerHTML = "empty";
    }
    else
    {
        location.href = './auth';
    }
}

GetStatus();