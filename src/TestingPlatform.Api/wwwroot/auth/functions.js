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
        document.getElementById("result").innerHTML = await response.json();
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
        document.getElementById("result").innerHTML = await response.json();
    }
}

async function LogoutFunction() {
    const response = await fetch("../api/v1/Users/Logout", {
        method: "GET"
    });
    if (response.ok === true) {
        document.getElementById("result").innerHTML = "empty";
    }
}