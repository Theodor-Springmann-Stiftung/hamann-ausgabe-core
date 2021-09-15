/*
    Funktion zum Einklappen der Kommentare der Seitenleiste, je nach verfügbarem Platz.
    Benötigt jQuery.
    (c) 2020 Theodor Springmann Stiftung Heidelberg
*/
$(function() {
    /* 
        Jede Zeile in der Seitenleiste wird ausgewählt.
        Voraussetzungen:
            - Marginalspalte hat die Klasse "commentColumn"
            - Kommentar in einer Zeile hat die Klasse "commBox"
            - Eine Zeile ohne Kommentar ist ein <br class="emptyline"/>
    */
    const sidebar = '.commentColumn';
    const comment = '.commBox';
    const emptyline = '.emptyline';

    /* 
        Hilfsvariablen
            - Funktion, um die Ergebnisse einer Query umzukehren.
            - Variable, in der die Anzahl verfügbaren Platzes gespeichert wird.
            - Größe eines REM
            - Zeilenhöhe in commBox
            - Padding von commBox
    */
    jQuery.fn.reverse = [].reverse;
    var emptylines_cnt = 0;
    var rem = parseFloat(getComputedStyle(document.documentElement).fontSize);
    var commBoxLineHeight = 1;
    var commBoxPaddingHeight = 0.2;

    /* Die Liste an Zeilen in der Seitenspalte wird rückwärts durchgegangen. */
    $(sidebar + ' ' + comment + ',' + emptyline).reverse().each(function(elem) {

        /* Fall 1: Es handelt sich um eine Leerzeile. Die Anzahl verfügbarer Leerzeilen wird erhöht. */
        if ($(this).hasClass("emptyline")) {
            emptylines_cnt++;
        }

        /* Fall 2: Es handelt sich um einen Kommentar, der evtl. gekürzt werden muss. */
        else {

           /* Von der Leerzeilenzahl bestimmte MÖGLICHE höhe des Divs berechnen
            bei der von mir gewählten Zeilenhöhe wird ab 5 fre ien Textzeilen eine extra Kommentarzeile frei*/
            if (emptylines_cnt >= 2) {
                bonusLines = Math.floor(emptylines_cnt / 2);
                emptylines_cnt = bonusLines + emptylines_cnt;
            }
            var possibleHeight = emptylines_cnt 
                                    * commBoxLineHeight 
                                    + commBoxLineHeight 
                                    + commBoxPaddingHeight;
            possibleHeight = Math.floor(possibleHeight * 100) / 100;

            var contentHeight = $(this).height();
            /*contentHeight in rem umrechnen*/
            contentHeight = contentHeight / rem;
            contentHeight = Math.ceil(contentHeight * 100) / 100;

            if (contentHeight > possibleHeight) {
                $(this).toggleClass("short");
                possibleHeight = String(possibleHeight) + "rem";
                $(this).css("height", possibleHeight);
            }
            emptylines_cnt = 0;
        }
    });


   $('button').on('click', function() {
    if (this.parentNode.parentNode.classList.contains("short")) { /*testen ob parrentnode kurzkommentar ist. Wenn JA:*/
        if (this.parentNode.parentNode.classList.contains("expanded")) { /*testen ob gerade expanded Wenn JA:*/
            this.parentNode.parentNode.classList.remove("expanded"); /*entferne expandeded*/
        } else { /*wenn parentnode nicht expanded ist*/
            $("button").each(function(i) {
                if (this.parentNode.parentNode.classList.contains("expanded")) {
                    this.parentNode.parentNode.classList.remove("expanded"); /*werden alle kommentare geschlossen*/
                }
            });
            this.parentNode.parentNode.classList.add("expanded"); /*und der aktuelle kommentar expandiert*/
       }
    } else {
        $("button").each(function(i) { /*wenn die parentnode nicht short ist*/
            if (this.parentNode.parentNode.classList.contains("expanded")) {
                this.parentNode.parentNode.classList.remove("expanded"); /*werden alle kommentare geschlossen*/
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
                childrenOfCommbox[i].style.width = "100%";
            } else if (i !== ntht_button && i !== 0) {
                childrenOfCommbox[i].classList.add("invisible");
                childrenOfCommbox[i].style.display = "none";
            }
        }

    });
});