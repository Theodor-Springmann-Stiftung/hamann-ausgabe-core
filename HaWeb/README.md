# Redesign der Hamann-Vebseite, drittes Major Upadte
Veränderungenen in der Funktionalität für den Benutzer
- Behutsames Redesign der Webseite:
    - Schrift wird minimal größer auf großen Bildschirmen
    - Redesign der Marginalspalte, sodass keine Überschneidungen vorkommen, gut für screenreader
    - Leichtes Redesign der Zusatzinformationen für einen Brief (Tradition, Edits) an etwas prominenterer Stelle
    - Anpassung der Webseite an Mobilgeräte und Tablets

- Suche im Brieftext wird gefixt
- Seite wird zugänglich für Menschen mit eingeschränktem Sehvermögen / Screenreadern
- Bibellinks in Bibelkommentaren

Veränderungen in der Funktionalität für die Forscher
- Online-Syntaxcheck für XML-Dateien
- Online-Cross-Dateien-Check (bsp. verweist jede Marginalie auf eine existierende Zeile)?
- Erstellung einer HAMANN.xml-Datei
- Hochladen der HAMANN.xml-Datei auf die Plattform

Veränderungen im Code
- Anpassung des Codes an .NET 6 (Kopieren / Einfügen)
- Umzug nach CSS Framework Tailwind 
- Code wird aufgeräumt und sortiert
- Leichtere Anpassungen an zukünftige Bedütfnisse (Wartungszeiten minimieren)
- Auslagerungen einzelner Einstellungen in Einstellungsdateien
- Code open source zugänglich machen?

Für diese Anpassungen sind in etwa 3-4 Monate eingeplant.
- Ladezeiten (???)
- PDF-Parser

hamann-werke.de
Startseite für die Briefausgebe / Werkausgabe. Unterschiedliche Menüs für die Ausgaben...

Briefe beim Namen

- GND Normdaten der Namen

TODO 1127 zu breit
TODO tabellen ok, ausser 939, 806 falsch geschachtelt: dort sind htabs geschachtelt
TODO 659 align center und align-right ueberschneidugn
TODO Kommentare und min-size von ha-lettertetx
TODO Word-wrap before align, tabs
TODO pills are not mobile friendly (hover / click)
TODO Evtl alignment von center / right an der letzten oder nächsten zeile
TODO Abhärten des Konstruktors von XMLRootDokument für von außerhalb platzierte Dokumente
TODO XML-Check im Client
TODO Lock für die Liste, Bzw ConcurretBag
TODO Up-Button
TODO Neue Forschungsliteratur einsenden