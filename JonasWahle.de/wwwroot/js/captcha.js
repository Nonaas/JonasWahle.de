function reloadHCaptcha() {
    if (window.hcaptcha) {
        console.log("Reloading hCaptcha...");
        hcaptcha.render(document.querySelector('.h-captcha'));
    } else {
        console.error("hCaptcha is not available!");
    }
}