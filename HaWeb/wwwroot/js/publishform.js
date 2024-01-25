const startup_publishform = function () {

  const LOCALPUBLISHSubmit = async function (oFormElement) {
    var fd = new FormData();
    document.getElementById("ha-publishfilelabel").style.pointerEvents = "none";
    document.getElementById("ha-lds-ellipsis-publish").style.display = "inline-block";
    document.getElementById("ha-publishmessage").style.opacity = "0";
    await fetch(oFormElement.action, {
      method: 'POST',
      headers: {
        'RequestVerificationToken': getCookie('RequestVerificationToken')
      }
    })
      .then(response => response.json())
      .then(json => {
        if ("Error" in json) {
          document.getElementById("ha-publishfilelabel").style.pointerEvents = "auto";
          document.getElementById("ha-lds-ellipsis-publish").style.display = "none";
          document.getElementById("ha-publishmessage").style.opacity = "1";
          document.getElementById("publish-result").value = json.Error;
        } else {
          document.getElementById("ha-publishfilelabel").style.pointerEvents = "auto";
          document.getElementById("ha-lds-ellipsis-publish").style.display = "none";
          document.getElementById("ha-publishmessage").style.opacity = "1";
          document.getElementById("publish-result").value = "Erfolg!";
          location.reload();
        }
      })
      .catch((e) => {
        document.getElementById("ha-publishfilelabel").style.pointerEvents = "auto";
        document.getElementById("ha-lds-ellipsis-publish").style.display = "none";
        document.getElementById("publish-result").value = "Keine Antwort. Bitte Seite neu laden!";
      })
  }

  var publishelement = document.getElementById("ha-publishform");
  var publishbutton = document.getElementById("ha-publishfilelabel");
  publishbutton.addEventListener("click", () => LOCALPUBLISHSubmit(publishelement));
};

startup_publishform();