const overlay = document.getElementById('loginOverlay');
const btnLoginPopup = document.querySelector('.btnLogin-popup');
const iconClose = document.querySelector('.icon-close');
const closeOverlay = document.getElementById('closeOverlay');
const loginLink = document.querySelector('.login-link');
const registerLink = document.querySelector('.register-link');
const wrapper = document.querySelector('.wrapper');
const inputFields = document.querySelectorAll('.form-box input');
const passwordToggle = document.getElementById('passwordToggle');
const passwordInput = document.getElementById('passwordInput');
const passwordToggle1 = document.getElementById('passwordToggle1');
const passwordInput1 = document.getElementById('passwordInput1');
const passwordToggle2 = document.getElementById('passwordToggle2');
const passwordInput2 = document.getElementById('passwordInput2');

passwordToggle.addEventListener('click', function () {
    const icon = passwordToggle.querySelector('ion-icon');
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        icon.name = 'eye-off-outline';
    } else {
        passwordInput.type = 'password';
        icon.name = 'eye-outline';
    }
});

passwordToggle1.addEventListener('click', function () {
    const icon = passwordToggle1.querySelector('ion-icon');
    if (passwordInput1.type === 'password') {
        passwordInput1.type = 'text';
        icon.name = 'eye-off-outline';
    } else {
        passwordInput1.type = 'password';
        icon.name = 'eye-outline';
    }
});

passwordToggle2.addEventListener('click', function () {
    const icon = passwordToggle2.querySelector('ion-icon');
    if (passwordInput2.type === 'password') {
        passwordInput2.type = 'text';
        icon.name = 'eye-off-outline';
    } else {
        passwordInput2.type = 'password';
        icon.name = 'eye-outline';
    }
});

function openOverlay() {
    overlay.style.display = 'flex';
    showLogin();
    clearInputFields();
}

function closeOverlayFunc() {
    overlay.style.display = 'none';
    clearInputFields();
}

function showLogin() {
    wrapper.classList.remove('active');
}

function showRegister() {
    wrapper.classList.add('active');
}

function clearInputFields() {
    inputFields.forEach((input) => {
        input.value = '';
        if (input.type === 'password') {
            input.type = 'password';
            const eyeIcon = input.parentElement.querySelector('.icon');
            eyeIcon.querySelector('ion-icon').name = 'eye-outline';
        }
    });
}

btnLoginPopup.addEventListener('click', () => {
    openOverlay();
    showLogin();
});

iconClose.addEventListener('click', closeOverlayFunc);
closeOverlay.addEventListener('click', closeOverlayFunc);
loginLink.addEventListener('click', showLogin);
registerLink.addEventListener('click', showRegister);

window.addEventListener('click', (event) => {
    if (event.target === overlay) {
        closeOverlayFunc();
    }
});



