$(document).ready(function () {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/notificationHub")
                .build();

            connection.on("ReceiveNotification", function (notificationData) {
                toastr.success(`<span class="toast-message">${notificationData.message}</span><ion-icon class="check-indicator-toaster" name="close-outline"></ion-icon>`);
                loadNotifications();
            });

            connection.start().then(function () {
                console.log('SignalR connection established');
                loadNotifications();
            }).catch(function (err) {
                console.error('SignalR connection error:', err.toString());
            });

            async function loadNotifications() {
                try {
                    const response = await fetch('/api/notification');

                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }

                    const notifications = await response.json();
                    const notificationsBody = document.getElementById('notificationsBody');
                    notificationsBody.innerHTML = '';

                    if (notifications.length > 0) {
                        for (const notification of notifications) {
                            const notificationElement = document.createElement('div');
                            notificationElement.classList.add('notification-item');

                            const [username, ...messageParts] = notification.message.split(' ');
                            const message = messageParts.join(' ');

                            const profileImageElement = document.createElement('img');
                            profileImageElement.src = notification.profileImageUrl;
                            profileImageElement.alt = `${username}'s profile picture`;
                            profileImageElement.classList.add('profile-image-notification');

                            const messageContainer = document.createElement('div');
                            messageContainer.classList.add('message-container');

                            const usernameElement = document.createElement('a');
                            usernameElement.textContent = username;
                            usernameElement.href = `/Profile/Overview/${notification.userId}`;
                            usernameElement.classList.add('username-link');

                            const messageAndTimeContainer = document.createElement('div');
                            messageAndTimeContainer.classList.add('message-time-container');

                            const messageElement = document.createElement('span');
                            messageElement.textContent = message;
                            messageElement.classList.add('message-text');

                            const followedAt = new Date(notification.createdAt);
                            const timeAgo = getTimeDifference(followedAt);

                            const timeAgoElement = document.createElement('span');
                            timeAgoElement.textContent = timeAgo;
                            timeAgoElement.classList.add('time-ago');

                            messageAndTimeContainer.appendChild(messageElement);
                            messageAndTimeContainer.appendChild(timeAgoElement);

                            messageContainer.appendChild(usernameElement);
                            messageContainer.appendChild(messageAndTimeContainer);

                            const followBackButton = document.createElement('button');
                            followBackButton.classList.add('follow-back-button');
                            followBackButton.onclick = () => followBack(notification.userId, followBackButton);

                            const isFollowing = await checkIfFollowing(notification.userId);
                            const isFollower = await checkIfFollower(notification.userId);

                            if (isFollowing && isFollower) {
                                followBackButton.textContent = 'Friends';
                                followBackButton.disabled = true;
                            } else if (!isFollowing && isFollower) {
                                followBackButton.textContent = 'Follow back';
                            }

                            notificationElement.appendChild(profileImageElement);
                            notificationElement.appendChild(messageContainer);
                            notificationElement.appendChild(followBackButton);
                            notificationsBody.appendChild(notificationElement);
                        }
                    } else {
                        notificationsBody.innerHTML = '<p>You don\'t have any notifications</p>';
                    }
                } catch (error) {
                    console.error('Error loading notifications:', error);
                }
            }

            async function checkIfFollowing(userId) {
                try {
                    const response = await fetch(`/api/followers/isFollowing/${userId}`);
                    const result = await response.json();
                    return result.isFollowing;
                } catch (error) {
                    console.error('Error checking if following:', error);
                    return false;
                }
            }

            async function checkIfFollower(userId) {
                try {
                    const response = await fetch(`/api/followers/isFollower/${userId}`);
                    const result = await response.json();
                    return result.isFollower;
                } catch (error) {
                    console.error('Error checking if follower:', error);
                    return false;
                }
            }

            function getTimeDifference(createdTime) {
                const now = new Date();
                const diffMs = now - new Date(createdTime);
                const diffMins = Math.floor(diffMs / 60000);
                const diffHours = Math.floor(diffMins / 60);
                const diffDays = Math.floor(diffHours / 24);
                const diffMonths = Math.floor(diffDays / 30);

                if (diffMins < 1) {
                    return 'just now';
                } else if (diffMins < 60) {
                    return diffMins + ' min. ago';
                } else if (diffHours < 24) {
                    return diffHours + ' hr. ago';
                } else if (diffDays < 30) {
                    return diffDays + ' day' + (diffDays > 1 ? 's' : '') + ' ago';
                } else {
                    return diffMonths + ' mo.' + (diffMonths > 1 ? 's' : '') + ' ago';
                }
            }

            async function followBack(userId, button) {
                try {
                    const response = await fetch('/api/followers/followback', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({ userId })
                    });

                    if (response.ok) {
                        button.textContent = 'Friends';
                        button.disabled = true;
                    } else {
                        console.error('Error following user');
                    }
                } catch (error) {
                    console.error('Error following user:', error);
                }
            }
        });