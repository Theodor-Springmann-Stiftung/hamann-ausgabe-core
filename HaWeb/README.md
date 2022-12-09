# Building and preparation

## Prep
To build the project install nodejs > 16.5 LTS. Install npm > 8.10.0. After that, do an `npm install` in the project directory to install the necessary dependencies. Nodejs is used for css scaffolding, this project uses `postcss`, with the `tailwindcss` CSS framework as a postcss plugin to only include used classes. Also, the `postcss-import` postcss-plugin is used to allow for compartmentalization of css files and the import statement to merge those files together (be careful of the order of commands in `postcss.config.js`!). `autoprefixer` and `css-nano` plugins are recommended at least for production builds since they provide cross-browser-compatibility and minification of file size for css files.

Dotnet 6.0.300 is currently used. To build the project, do a `dotnet restore` and collect the `Microsoft.FeatureManagement.AspNetCore` nuget-package which is used to enable feauture-flags in `appsettings.json`. Some routes, such as the admin area of the project will only be mapped if certain flags are present and set to true. 

Also, this project requires two other projects `HaDocumentV6` (for reading in the file into convenient to use models) and `HaXMLReader` (for forward parsing elements such as letters, comments, traditions and marginals in an HTML transform). They have no dependencies (apart from each other and `.NET 6`) and are  build and linked at build time automatically.

## Building the project 

`dotnet build Tailwind.csproj` or `npm run css_build`

to build the necessary `output.css`-File.

`dotnet build HaWeb.csproj`

to build the Website. Please do consider the order of these commands.

Don't forget to place a valid `Hamann.xml`-File in the root of the build to provide a starting and fallback XML-file.

Note that nodejs is used only as a build tool for css (and possibly in the future javascript) processing. To host the website there is no node needed.

## Development tools

Run

`dotnet watch run` and

`npm run watch`

in seperate terminals to watch for specific file changes in .css / .js / .cshtml / .json or .cs files and to rebuild the css-Files and the app automatically on change.

There is a chance you need to set the Environment Variable to 'Development' in Windows, do that with `$Env:ASPNETCORE_ENVIRONMENT = "Development"`.

Recommended vscode plugins include the XML Tools, c#, Tailwind CSS IntelliSense & TODO Tree.

## Redesign der Hamann-Vebseite, drittes Update
Veränderungenen in der Funktionalität für den Benutzer
- Behutsames Redesign der Webseite:
    - Schrift wird minimal größer auf großen Bildschirmen
    - Redesign der Marginalspalte, sodass keine Überschneidungen vorkommen, gut für screenreader
    - Leichtes Redesign der Zusatzinformationen für einen Brief (Tradition, Edits) an etwas prominenterer Stelle
    - Anpassung der Webseite an Mobilgeräte und Tablets
- Suche im Brieftext wird gefixt
- Seite wird zugänglich für Menschen mit eingeschränktem Sehvermögen / Screenreadern
- Bibellinks in Bibelkommentaren
- Verbesserte Ladezeiten
- Startseite für die Briefausgebe / Werkausgabe. Unterschiedliche Menüs für die Ausgaben
- URL für die Register hat sich geändert, bleibt aber weiter unter `/Supplementa/` zugängig.
- Ebenso alle anderen URLs für die Briefausgabe
- Anzeige Briefe beim Namen (?)
- GeoCities und Personen-Verweise (?)

Veränderungen in der Funktionalität für die Redakteure
- Online-Syntaxcheck für XML-Dateien (Noch nicht implementiert)
- Online-Cross-Dateien-Check (bsp. verweist jede Marginalie auf eine existierende Zeile) (Noch nicht implementiert)
- Erstellung einer HAMANN.xml-Datei, hochladen der HAMANN.xml-Datei auf die Plattform

Veränderungen im Code
- Anpassung des Codes an .NET 6
- Umzug nach CSS Framework Tailwind 
- Code ist aufgeräumt und sortiert
- Leichtere Anpassungen an zukünftige Bedürfnisse (Wartungszeiten minimieren)
- Auslagerungen einzelner Einstellungen in Einstellungsdateien
- Code wird open source zugänglich

Ästhetisch unzufriedenstellend, aber funktional:
B TODO pills are not mobile friendly (hover / click), Pills allgemein Ästhetik
D TODO High Contrast Mode manchmal komisch
D TODO Kein High Contrast Mode für den Upload
D TODO High Contrast Mode: Kursiv und ausgegraut (Herausgeberanmerkungen) schwer sichtbar
D TODO 400: Traditions nicht genug Abstand

Technische Details:
D TODO Move ILibrary -> neuer Parser
C TODO Syntax-Check
B TODO Datum im Footer Edierdatum der Hamann-Datei
A TODO Fußnoten in Editionsgeschichte als Marginalkommentare

Vor dem internen release:
A TODO Jahreszahlen auf der Startseite
B TODO Suchergebnisse beschränken
B TODO Mobile Menüs bei der Seitennavigation (Jahrszahlen, Buchstabenindex usw)
C TODO Traditions durchsuchen

Liste für Janina/Luca:
tabellen ok, ausser 939

KOmmentare verschobem 202 Anhang

Evtl finetuning von note

- Text auf der Startseite
- Stellenkommentare in extra Tab

- click event does not work in iOS
- rerende marginals on tab switch