const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7097/chatHub", {
        accessTokenFactory: () => sessionStorage.getItem('JWTToken')
    })
    .withAutomaticReconnect()
    .build();

connection.start()
    .then(() => console.log("Connected to SignalR hub"))
    .catch(err => console.error("SignalR Connection Error: ", err));

connection.on("ReceiveMessage", (user, message) => {
    const messageList = document.getElementById("messageList");
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    messageList.appendChild(li);
});

async function sendMessage(userId, messageContent) {
    if (!messageContent) return;

    const token = sessionStorage.getItem('JWTToken');
    if (!token) {
        alert("Bạn cần đăng nhập lại để tiếp tục sử dụng chat.");
        return;
    }

    try {
        // 1. Gửi tin nhắn qua API
        const response = await fetch("https://localhost:7097/api/chat/send", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                receiverId: 1,
                messageContent: messageContent
            })
        });

        if (!response.ok) {
            throw new Error("Unauthorized access - Please log in again.");
        }

        const data = await response.json();
        if (data.isSuccess) {
            // 2. Nếu API thành công, gửi tin nhắn qua SignalR
            await connection.invoke("SendMessage", "User", messageContent);
            document.getElementById("messageInput").value = "";
        } else {
            alert("Error sending message");
        }
    } catch (error) {
        console.error("Error:", error);
    }
}