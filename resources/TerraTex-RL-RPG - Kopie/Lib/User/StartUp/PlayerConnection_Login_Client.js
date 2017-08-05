let browser;
let nickname;

API.onServerEventTrigger.connect(function(eventName, args) {
    if (eventName === "startLogin") {
        API.setCanOpenChat(false);
        const resolution = API.getScreenResolution();
        const width = Math.round(resolution.Width < 690 ? resolution.Width : 690);
        const height = Math.round(resolution.Height < 390 ? resolution.Height : 390);

        browser = API.createCefBrowser(width, height, true);
        API.waitUntilCefBrowserInit(browser);

        const x = (resolution.Width - width) / 2;
        const y = (resolution.Height - height) / 2;

        API.setCefBrowserPosition(browser, x, y);
        API.setCefBrowserHeadless(browser, false);
        API.loadPageCefBrowser(browser, 'UI/Register_Login_Profile/Login.html', false);
        API.showCursor(true);
        API.waitUntilCefBrowserLoaded(browser);
        nickname = args[0];
    }
});

function sendMeNickname() {
    browser.call("setPlayerNickname", nickname);
}

function sendLogin(regData) {
    API.destroyCefBrowser(browser);
    API.showCursor(false);
    API.setCanOpenChat(true);
    API.triggerServerEvent("onClientStartLogin", regData);
}