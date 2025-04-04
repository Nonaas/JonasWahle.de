function reloadHCaptcha() {
    if (window.hcaptcha) {
        hcaptcha.render(document.querySelector('.h-captcha'));
    }
}

function validateCaptcha() {
    var captchaResponse = document.querySelector('textarea[name="h-captcha-response"]');
    if (!captchaResponse || captchaResponse.value.trim() === "") {
        document.getElementById("captcha-error").style.display = "block";
        return false;
    }
    return true;
}