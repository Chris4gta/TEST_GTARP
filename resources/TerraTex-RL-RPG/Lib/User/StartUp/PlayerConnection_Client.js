let browser;
function setFingerPrint(result) {
    API.triggerServerEvent("onClientResourceStarted", result);
    API.destroyCefBrowser(browser);
}

API.onResourceStart.connect(function () {
    API.setCanOpenChat(false);
    browser = API.createCefBrowser(1,1, true);
    API.waitUntilCefBrowserInit(browser);
    API.setCefBrowserHeadless(browser, true);
    API.loadPageCefBrowser(browser, 'UI/Register_Login_Profile/GetFingerprint.html', false);
});

