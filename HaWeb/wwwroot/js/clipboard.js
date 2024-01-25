
const startup_clipboard = function () {
  document.addEventListener('copy', function (e) {
    var e = navigator.clipboard.read();

    var text = window.getSelection().toString();
    e.clipboardData.setData('text/plain', text);
    e.preventDefault();
  });
}


startup_clipboard();