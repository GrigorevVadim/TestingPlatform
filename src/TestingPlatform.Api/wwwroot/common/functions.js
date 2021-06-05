async function LogoutFunction(address) {
    const response = await fetch("../api/v1/Users/Logout", {
        method: "GET"
    });
    if (response.ok === true) {
        location.href = address + '/auth';
    }
}

async function ReturnToMainPageFunction() {
    location.href = '../';
}