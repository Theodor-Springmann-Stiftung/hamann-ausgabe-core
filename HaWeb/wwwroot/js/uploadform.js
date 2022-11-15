const dropHandler = function (formelement, ev, dropzone) {
    ev.preventDefault();
    if (ev.dataTransfer.items) {
      if (ev.dataTransfer.items[0].kind === 'file') {
        var file = ev.dataTransfer.items[0].getAsFile();
        UPLOADSubmit(formelement, file);
      } else {
        var file = ev.dataTransfer.files[0];
        UPLOADSubmit(formelement, file);
      }
    }
  }

  const dragOverHandler = function (ev, dropzone) {
    ev.preventDefault();
  }

  const dragLeaveHander = function (ev, dropzone) {
    ev.preventDefault();
  }

  const dragEnterHandler = function (ev, dropzone) {
    ev.preventDefault();
  }

  const UPLOADSubmit = async function (oFormElement, file = null) {
    var fd = new FormData();
    if (file !== null) fd.append("file", file);
    else fd = new FormData(oFormElement);
    document.getElementById("dropzone").style.pointerEvents = "none";
    document.getElementById("ha-lds-ellipsis-upload").style.display = "inline-block";
    document.getElementById("ha-uploadmessage").style.opacity = "0";
    await fetch(oFormElement.action, {
        method: 'POST',
        headers: {
          'RequestVerificationToken': getCookie('RequestVerificationToken')
        },
        body: fd
      })
      .then(response => response.json())
      .then(json => {
        if ("Error" in json) {
          document.getElementById("dropzone").style.pointerEvents = "auto";
          document.getElementById("ha-lds-ellipsis-upload").style.display = "none";
          document.getElementById("ha-uploadmessage").style.opacity = "1";
          oFormElement.elements.namedItem("upload-result").value = json.Error;
        } else {
          document.getElementById("dropzone").style.pointerEvents = "auto";
          document.getElementById("ha-lds-ellipsis-upload").style.display = "none";
          oFormElement.elements.namedItem("upload-result").value = "Erfolg!";
          if ("Prefix" in json[0]) {
            document.getElementById("dropzone").style.pointerEvents = "auto";
            document.getElementById("ha-lds-ellipsis-upload").style.display = "none";
            window.location.replace("/Admin/Upload/" + json[0].Prefix);
          }
        }
      })
      .catch ((e) => { 
        document.getElementById("dropzone").style.pointerEvents = "auto";
        document.getElementById("ha-lds-ellipsis-upload").style.display = "none";
        document.getElementById("ha-uploadmessage").style.opacity = "1";
        oFormElement.elements.namedItem("upload-result").value = "Keine Antwort. Bitte Seite neu laden!";
      })
  }

var submitelement = document.getElementById("file");
var formelement = document.getElementById("uploadForm");
var dropzone = document.getElementById("dropzone");
submitelement.addEventListener("change", () => UPLOADSubmit(formelement));
dropzone.addEventListener("drop", (ev) => dropHandler(formelement, ev, dropzone));
dropzone.addEventListener("dragover", (ev) => dragOverHandler(ev, dropzone));
dropzone.addEventListener("dragleave", (ev) => dragLeaveHander(ev, dropzone));
dropzone.addEventListener("dragenter", (ev) => dragEnterHandler(ev, dropzone));