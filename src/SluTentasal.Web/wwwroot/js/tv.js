// Beteenden för TV-visning: varning vid tappad kontakt, helskärm och skärmsläckarskydd.
(function () {
    "use strict";

    // --- Tappad kontakt -------------------------------------------------
    // Klockan pollas varje sekund, så ett misslyckat anrop märks direkt.
    // Lösenordet ligger kvar, men klockan dimmas och en varning visas.
    function setOffline(offline) {
        document.body.classList.toggle("is-offline", offline);
    }

    document.body.addEventListener("htmx:sendError", function () { setOffline(true); });
    document.body.addEventListener("htmx:responseError", function () { setOffline(true); });
    document.body.addEventListener("htmx:afterRequest", function (e) {
        if (e.detail.successful) {
            setOffline(false);
        }
    });

    // --- Lugna byten av lösenordsplattan -------------------------------
    // Plattan tonas bara in när läget ändras (väntläge → lösenord osv.).
    // Vid samma läge byts innehållet utan övertoning, så pollningen syns inte.
    function panelState(html) {
        var match = /class="panel (?:steady )?(\w+)/.exec(html);
        return match ? match[1] : null;
    }

    document.body.addEventListener("htmx:beforeSwap", function (e) {
        var target = e.detail.target;
        if (!target.classList.contains("password") || !e.detail.shouldSwap) {
            return;
        }

        var current = target.querySelector(".panel");
        var incoming = e.detail.serverResponse;
        if (current && panelState(current.outerHTML) === panelState(incoming)) {
            e.detail.serverResponse = incoming.replace('class="panel ', 'class="panel steady ');
        }
    });

    // --- Helskärm -------------------------------------------------------
    document.addEventListener("click", function () {
        if (document.fullscreenElement) {
            document.exitFullscreen().catch(function () { });
        } else if (document.documentElement.requestFullscreen) {
            document.documentElement.requestFullscreen().catch(function () { });
        }
    });

    // --- Håll skärmen vaken -------------------------------------------
    var wakeLock = null;

    function requestWakeLock() {
        if (!("wakeLock" in navigator) || document.visibilityState !== "visible") {
            return;
        }
        navigator.wakeLock.request("screen")
            .then(function (lock) { wakeLock = lock; })
            .catch(function () { /* stöds inte eller nekades – inget att göra */ });
    }

    document.addEventListener("visibilitychange", function () {
        if (document.visibilityState === "visible" && (wakeLock === null || wakeLock.released)) {
            requestWakeLock();
        }
    });

    requestWakeLock();
})();
