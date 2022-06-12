# Building and preparation

## Prep
To build the project install nodejs > 16.5 LTS. Install npm > 8.10.0. After that, do an `npm install` in the project directory to install the necessary dependencies. Nodejs is used for css scaffolding, this project uses `postcss`, with the `tailwindcss` CSS framework as a postcss plugin to only include used classes. Also, the `postcss-import` postcss-plugin is used to allow for compartmentalization of css files and the import statement to merge those files together (be careful of the order of commands in `postcss.config.js`!). `autoprefixer` and `css-nano` plugins are recommended at least for production builds since they provide cross-browser-compatibility and minification of file size for css files.

Dotnet 6.0.300 is currently used. To build the project, do a `dotnet restore` and collect the `Microsoft.FeatureManagement.AspNetCore` nuget-package which is used to enable feauture-flags in `appsettings.json`. Some routes, such as the admin area of the project will only be mapped if certain flags are present and set to true. 

Also, this project requires two other projects `HaDocumentV6` (for reading in the file into convenient to use models) and `HaXMLReader` (for forward parsing elements such as letters, comments, traditions and marginals in an HTML transform). They have no dependencies (apart from each other and `.NET 6`) and are build at build time automatically.

## Building the project 

`dotnet build Tailwind.csproj` or `npm run css_build`

to build the necessary `output.css`-File.

`dotnet build HaWeb.csproj`

to build the App. Please do consider the order of these commands.

Don't forget to place a valid `Hamann.xml`-File in the root of the build to provide a starting and fallback XML-file.

Note that nodejs is used only as a build tool for css (and possibly in the future javascript) processing. To host the website there is no node needed.

## Development tools

Run

`dotnet watch --verbose --project Tailwind.csproj build -- Tailwind.csproj` and

`dotnet watch --verbose --project HaWeb.csproj -- run --project HaWeb.csproj`

in seperate terminals to watch for specific file changes in .css / .js / .cshtml / .json or .cs files and to rebuild the css-Files and the app automatically on change.

Recommended vscode plugins include the XML Tools, Prettier, c#, Nuget Gallery, Tailwind CSS IntelliSense & TODO Tree.

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
- Anzeige Briefe beim Namen (?)
- GeoCities und Personen-Verweise (?)

Veränderungen in der Funktionalität für die Forscher
- Online-Syntaxcheck für XML-Dateien
- Online-Cross-Dateien-Check (bsp. verweist jede Marginalie auf eine existierende Zeile)
- Erstellung einer HAMANN.xml-Datei
- Hochladen der HAMANN.xml-Datei auf die Plattform

Veränderungen im Code
- Anpassung des Codes an .NET 6 (Kopieren / Einfügen)
- Umzug nach CSS Framework Tailwind 
- Code wird aufgeräumt und sortiert
- Leichtere Anpassungen an zukünftige Bedürfnisse (Wartungszeiten minimieren)
- Auslagerungen einzelner Einstellungen in Einstellungsdateien
- Code open source zugänglich machen?


TODO 1127 zu breit
TODO tabellen ok, ausser 939, 806 falsch geschachtelt: dort sind htabs geschachtelt
TODO 659 align center und align-right ueberschneidugn
TODO Word-wrap before align, tabs
TODO pills are not mobile friendly (hover / click)
TODO Evtl alignment von center / right an der letzten oder nächsten zeile
TODO Abhärten des Konstruktors von XMLRootDokument für von außerhalb platzierte Dokumente
TODO XML-Check im Client
TODO Lock für die Liste, Bzw ConcurretBag
TODO 516A david friedlaender in den traditions