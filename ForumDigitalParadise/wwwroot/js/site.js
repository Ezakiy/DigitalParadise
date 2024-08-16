const rowPostCreate = document.querySelector(".row-postcreate");

function changeBorderColor() {
    rowPostCreate.style.borderColor = "#cecece";
}

textarea.addEventListener("click", changeBorderColor);

document.body.addEventListener("click", function (event) {
    if (event.target !== textarea) {
        rowPostCreate.style.borderColor = "#8686868c";
    }
});

function updateFollowerCount(userId, change) {
    const userProfileElement = document.querySelector(`.profile-info[data-user-id='${userId}']`);
    if (userProfileElement) {
        const followerCountElement = userProfileElement.querySelector('.follower-count p');
        const currentCount = parseInt(followerCountElement.textContent);
        followerCountElement.textContent = currentCount + change;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const input = document.querySelector(".searchbar-MyFollowers");
    const firstDiv = document.querySelector(".Myfollowers-TitleInput");
    const followerItems = document.querySelectorAll(".followers-item");

    input.addEventListener("focus", function () {
        firstDiv.classList.add("focused");
    });

    input.addEventListener("blur", function () {
        firstDiv.classList.remove("focused");
    });

    input.addEventListener("input", function () {
        const filter = input.value.toLowerCase();
        console.log("Filter value:", filter);  // Debug log
        followerItems.forEach(function (item) {
            const followerName = item.querySelector(".follower-name").textContent.toLowerCase();
            console.log("Checking follower:", followerName);  // Debug log
            if (followerName.includes(filter)) {
                item.style.display = "";
            } else {
                item.style.display = "none";
            }
        });
    });
});

async function MyFollowersfollowUser(followerId, button) {
    console.log('Attempting to follow user:', followerId);
    try {
        const response = await fetch('/api/MyFollowers/follow', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ followerId: followerId })
        });
        const result = await response.json();
        if (result.success) {
            button.textContent = 'Unfollow';
            button.onclick = function () { MyFollowersunfollowUser(followerId, button); };
            button.classList.remove('follow-back-button-myfollowers');
            button.classList.add('unfollow-button-myfollowers');
            const followerCount = document.getElementById('followerCount');
            followerCount.innerText = parseInt(followerCount.innerText) + 1;
        } else {
            alert('Failed to follow the user: ' + result.message);
        }
    } catch (error) {
        console.error('Error following user:', error);
    }
}

async function MyFollowersunfollowUser(followerId, button) {
    console.log('Attempting to unfollow user:', followerId);
    try {
        const response = await fetch('/api/MyFollowers/unfollow', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ followerId: followerId })
        });
        const result = await response.json();
        if (result.success) {
            button.textContent = 'Follow back';
            button.onclick = function () { MyFollowersfollowUser(followerId, button); };
            button.classList.remove('unfollow-button-myfollowers');
            button.classList.add('follow-back-button-myfollowers');
            const followerCount = document.getElementById('followerCount');
            followerCount.innerText = parseInt(followerCount.innerText) - 1;
        } else {
            alert('Failed to unfollow the user: ' + result.message);
        }
    } catch (error) {
        console.error('Error unfollowing user:', error);
    }
}



