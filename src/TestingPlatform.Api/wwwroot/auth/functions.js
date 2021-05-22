async function LoginFunction() {
    const response = await fetch("../api/v1/Users/Login", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            Login: document.getElementById("login").value,
            Password: document.getElementById("pass").value
        })
    });
    if (response.ok === true) {
        location.href = '../';
    }
}

async function RegFunction() {
    const response = await fetch("../api/v1/Users/Register", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            Login: document.getElementById("login").value,
            Password: document.getElementById("pass").value
        })
    });
    if (response.ok === true) {
        location.href = '../';
    }
}