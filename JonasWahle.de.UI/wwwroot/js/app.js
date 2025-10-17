function setCookie(key, value, expires) {
    const date = new Date();
    date.setTime(date.getTime() + (expires * 24 * 60 * 60 * 1000));
    const expiresString = "expires=" + date.toUTCString();
    document.cookie = key + "=" + value + ";" + expiresString + ";path=/";
}

function getCookie(key) {
    const name = key + "=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookieArray = decodedCookie.split(';');
    
    for (let i = 0; i < cookieArray.length; i++) {
        let cookie = cookieArray[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(name) === 0) {
            return cookie.substring(name.length, cookie.length);
        }
    }
    return "";
}

function deleteCookie(key) {
    document.cookie = key + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}

function downloadQrCodeImage(qrCodeElementId) {
    const qrCodeElement = document.querySelector(qrCodeElementId);

    if (!qrCodeElement) return;

    // Get SVG
    const svgData = new XMLSerializer().serializeToString(qrCodeElement);

    // Convert to base64
    const svgBase64 = btoa(unescape(encodeURIComponent(svgData)));
    const svgUrl = `data:image/svg+xml;base64,${svgBase64}`;

    const image = new Image();
    image.onload = function () {
        const canvas = document.createElement("canvas");
        const size = qrCodeElement.clientWidth;

        canvas.width = size;
        canvas.height = size;

        const ctx = canvas.getContext("2d");
        ctx.drawImage(image, 0, 0, size, size);

        const dataUrl = canvas.toDataURL("image/png");
        const link = document.createElement("a");
        link.href = dataUrl;
        link.download = "qrcode.png";
        link.click();
    };

    image.src = svgUrl;
};