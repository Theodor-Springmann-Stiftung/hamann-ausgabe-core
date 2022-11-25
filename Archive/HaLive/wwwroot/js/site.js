// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function() {
    var commentColum = document.querySelectorAll("div.commentColumn");
    var nodeList = Array.from(commentColum[0].querySelectorAll("div.commBox, br.emptyline"));
    var nodeliLength = nodeList.length;
    var brcounter = 0;
    /* im Folgenden erstelle ich ein dictionary (map) mit der Zeilennummer des Kommentars als Schüssel und den Folgezeilen als Wert.
    Da ich leider zu dumm bin, um es anders zu machen, geht die Liste von der höchsten Zeilennummer zu nidrigsten. Desswegen referenziere ich später die Kommentare durch ihre ID=Zeilennummer */
    var commentLine_emptyLines = new Map();
    for (var i = nodeliLength; i > 0; i--) {
        if (nodeList[i - 1] == '[object HTMLBRElement]') {
            brcounter++;
        } else if (nodeList[i - 1] == '[object HTMLDivElement]') {
            commentLine_emptyLines.set(i, brcounter);
            brcounter = 0;
        }
    }
    /*dann stelle ich die höhe jedes Div.commBox so ein, dass die Höhe des Inhalts realisiert wird, wenn genug Platz ist.
    Wenn zu wenig Platz ist, wird das Div so groß wie möglich gemacht.*/
    $(".commBox").each(function(i) {
        /*Zahl der leeren Folgezeilen über Zeilenzahl des Kommentars bekommen*/
        var emptyLines = commentLine_emptyLines.get(parseInt(this.id));
            /* Von der Leerzeilenzahl bestimmte MÖGLICHE höhe des Divs berechnen
            bei der von mir gewählten Zeilenhöhe wird ab 5 (nach jetztiger einstellung ab 6) Freien Textzeilen eine extra Kommentarzeile frei*/
        if (emptyLines >= 4) {
            bonusLines = Math.floor(emptyLines / 4);
            emptyLines = bonusLines + emptyLines;
        }
        var possibleHeight = emptyLines * 1.1 + 1.1 + 0.2;
        possibleHeight = Math.floor(possibleHeight * 100) / 100;
        console.log(possibleHeight);
        var contentHeight = $(this).height();
        /*contentHeight in rem umrechnen*/
        contentHeight = contentHeight / parseFloat(getComputedStyle(document.documentElement).fontSize);
        contentHeight = Math.ceil(contentHeight * 100) / 100;
        console.log(contentHeight);
        if (contentHeight < possibleHeight) {
            possibleHeight = Math.ceil((contentHeight + 0.2) * 100) / 100;
        } else if (contentHeight > possibleHeight) {
            $(this).toggleClass("short");
        } else {
            possibleHeight = contentHeight;
        }

        possibleHeight = String(possibleHeight) + "rem";
        $(this).css("height", possibleHeight);
    });

    $("button").each(function(i) { /*wenn die parentnode nicht short ist*/
        if (this.parentNode.parentNode.classList.contains("short")) {} else {
            this.firstChild.data = "▷";
        }
    });

    $('button').on('click', function() {
        if (this.parentNode.parentNode.classList.contains("short")) { /*testen ob parrentnode kurzkommentar ist. Wenn JA:*/
            if (this.parentNode.parentNode.classList.contains("expanded")) { /*testen ob gerade expanded Wenn JA:*/
                this.parentNode.parentNode.classList.remove("expanded"); /*entferne expandeded*/
                this.firstChild.data = "▶ ";
            } else { /*wenn parentnode nicht expanded ist*/
                $("button").each(function(i) {
                    if (this.parentNode.parentNode.classList.contains("expanded")) {
                        this.parentNode.parentNode.classList.remove("expanded"); /*werden alle kommentare geschlossen*/
                        this.firstChild.data = "▶ ";
                    }
                });
                this.parentNode.parentNode.classList.add("expanded"); /*und der aktuelle kommentar expandiert*/
                this.firstChild.data = "▲";
            }
        } else {
            $("button").each(function(i) { /*wenn die parentnode nicht short ist*/
                if (this.parentNode.parentNode.classList.contains("expanded")) {
                    this.parentNode.parentNode.classList.remove("expanded"); /*werden alle kommentare geschlossen*/
                    this.firstChild.data = "▶ ";
                }
            });
        }
    });

    $('button').on('click', function() {
        var ntht_button = 1;
        $(this).prevAll().each(function(i) {
            ++ntht_button
        });
        var commBox = this.parentNode.parentNode;
        var childrenOfCommbox = commBox.children
        for (var i = 0; i < childrenOfCommbox.length; i++) {
            if (i == ntht_button) {
                childrenOfCommbox[i].classList.remove("invisible");
                childrenOfCommbox[i].style.display = "block";
            } else if (i !== ntht_button && i !== 0) {
                childrenOfCommbox[i].classList.add("invisible");
                childrenOfCommbox[i].style.display = "none";
            }
        }

    });
});