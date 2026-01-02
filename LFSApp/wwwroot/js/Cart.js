

function updateCartCount(newCount) {
    document.getElementById("cartCount").textContent = newCount;
}



document.addEventListener("DOMContentLoaded", function () {
    let result = document.getElementById("response").value;
    if (result === "true") {
        Toastify({
            text: "✅ Saved Successfully!",
            duration: 3000,
            newWindow: true,
            close: true,
            gravity: "top", // `top` or `bottom`
            position: "right", // `left`, `center` or `right`
            stopOnFocus: true, // Prevents dismissing of toast on hover
            style: {
                
                background: "linear-gradient(45deg, #d4a567, #d4a574)",
             
            },
            onClick: function () { } // Callback after click
        }).showToast();
    }

    
});
