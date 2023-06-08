// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const dateTimeSpans = document.querySelectorAll(".date-time-local");
for (const element of dateTimeSpans) {
    element.classList.remove("date-time-local");
    const culture = element.getAttribute("culture") ?? "en-US";
    element.textContent = (new Date(element.textContent)).toLocaleString(culture);
}

const dateTimeLocalInputs = document.querySelectorAll('input[type="datetime-local"]');
for (const element of dateTimeLocalInputs) {
    if (!element instanceof HTMLInputElement) continue;
    if (!element.value) continue;
    const date = new Date(element.value + "Z");
    const timeZoneOffset = date.getTimezoneOffset() * 60000;
    const adjustedDate = new Date(date.getTime() - timeZoneOffset);

    let isoString = adjustedDate.toISOString();
    if (isoString.length > 0) {
        isoString = isoString.substring(0, isoString.length - 2);
    }
    element.value = isoString;
    element.setAttribute("step", "0.001");
}

const forms = document.getElementsByTagName("form");
for (const form of forms) {
    form.addEventListener('submit', event => {
        event.preventDefault();

        const inputs = form.querySelectorAll('input[type="datetime-local"]');

        for (const input of inputs) {
            if (!input instanceof HTMLInputElement) continue;
            if (!input.value) continue;
            const date = new Date(input.value);
            const timeZoneOffset = date.getTimezoneOffset();
            const adjustedDate = new Date(date.getTime() - timeZoneOffset);
            const isoString = adjustedDate.toISOString();
            input.value = isoString.substring(0, isoString.length - 2);
        }

        form.submit();
    });
}