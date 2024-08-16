const dropdownBtn = document.querySelector('.dropdown-btn');
const userDropdown = document.querySelector('.user-dropdown-list');

dropdownBtn.addEventListener('click', (event) => {
    userDropdown.classList.toggle('active-userdropdown');
    event.stopPropagation(); 
});

document.body.addEventListener('click', (event) => {
    if (!userDropdown.contains(event.target) && !dropdownBtn.contains(event.target)) {
        userDropdown.classList.remove('active-userdropdown');
    }
});



