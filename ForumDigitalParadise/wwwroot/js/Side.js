const menuIcons = document.querySelectorAll('.menu-icon');
const sidenavOverlay = document.getElementById('sidenavOverlay');
const closeSidenav = document.getElementById('closeSidenav');
const sidenavContent = document.getElementById('sidenavContent');
let isManuallyToggled = false;

document.getElementById('sidenavOverlay').addEventListener('click', () => {
    closeSidenavFunc();
});

function toggleSidenav() {
    menuIcons.forEach(icon => {
        if (sidenavOverlay.style.display === 'none' || sidenavOverlay.style.display === '') {
            openSidenav();
            isManuallyToggled = true;
        } else {
            closeSidenavFunc();
            isManuallyToggled = false; 
        }
    });
}

menuIcons.forEach(icon => {
    icon.addEventListener('click', toggleSidenav);
});

function openSidenav() {
    document.body.classList.add('sidenav-open');
    sidenavOverlay.style.display = 'block';
    sidenavContent.style.display = 'inline-block';
}

function closeSidenavFunc() {
    document.body.classList.remove('sidenav-open');
    sidenavOverlay.style.display = 'none';
    sidenavContent.style.display = 'none';
}

closeSidenav.addEventListener('click', closeSidenavFunc);

window.addEventListener('click', (event) => {
    if (!sidenavContent.contains(event.target) && !sidenavOverlay.contains(event.target)) {
        closeSidenavFunc();
    }
});

function checkWindowWidth() {
    if (window.innerWidth > 1366 && sidenavOverlay.style.display === 'none') {
        openSidenav();
    } else if (window.innerWidth <= 1366 && !isManuallyToggled) {
        closeSidenavFunc();
    }
}

window.onload = checkWindowWidth;

window.addEventListener('resize', checkWindowWidth);
