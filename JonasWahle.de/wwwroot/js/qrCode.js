window.downloadQrCodeImage = function (qrCodeElementId) {
    const qrCodeElement = document.querySelector(qrCodeElementId);

    if (!qrCodeElement) return;

    // Get the SVG markup
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