# Building and preparation

## Prep
To build the project install nodejs > 16.5 LTS. Install npm > 8.10.0. After that, do an `npm install` in the project directory to install the necessary dependencies. Nodejs is used for css and javascript scaffolding, this project uses `vite` and `postcss`, with the `tailwindcss` CSS framework as a postcss plugin to only include used classes (be careful of the order of commands in `postcss.config.js`!). `autoprefixer` and `css-nano` plugins are recommended at least for production builds since they provide cross-browser-compatibility and minification of file size for css files.

Dotnet 6 is currently used. To build the project, do a `dotnet restore` and collect the `Microsoft.FeatureManagement.AspNetCore` nuget-package which is used to enable feauture-flags in `appsettings.json`. Some routes, such as the admin area of the project will only be mapped if certain flags are present and set to true. 

Also, this project requires two other projects `HaDocumentV6` (for reading in the file into convenient to use models) and `HaXMLReader` (for forward parsing elements such as letters, comments, traditions and marginals in an HTML transform). They have no dependencies (apart from each other and `.NET 6`) and are build and linked at build time automatically.

## Building the project 

`npm run build`

to build the necessary `wwwroot/dist/styles.css` & `wwwroot/dist/scripts.js`-Files.

`dotnet build HaWeb.csproj`

to build the Website. Please do consider the order of these commands.

Don't forget to place a valid `Hamann.xml`-File in the root of the build to provide a starting and fallback XML-file.

Note that nodejs is used only as a build tool for css and javascript. To host the website there is no node needed.

## Development tools

Set the `DOTNET_ENVIRONMENT` variable to `Development`. Run

`dotnet watch run` and

`npm run dev` or 

`bun run dev`

in seperate terminals to watch for specific file changes in .css / .js / .cshtml / .json or .cs files and to rebuild the css-Files and the app automatically on change.

There is a chance you need to set the Environment Variable to 'Development' in Windows, do that with `$Env:ASPNETCORE_ENVIRONMENT = "Development"`.

Recommended vscode plugins include the XML Tools, c#, Tailwind CSS IntelliSense & TODO Tree.

## Releas

For a Linux server run:

`dotnet publish --runtime linux-x64 -c Release`

In the appropriate settings (set the `DOTNET_ENVIRONMENT` variable on the server to `Staging` (development.hamann-ausgabe.de) or `Production` (hamann-ausgabe.de)) set the variables for

- HamannFileStoreLinux
- WorkingTreePathLinux
- BareRepositoryPathLinux
- RepositoryBranch
- RepositoryURL

Absolute paths, sadly, are reqired. Create Folders if neccessary. Writing rights are required for `HamannFileStoreLinux` (to save the state). The `BareRepositoryPath` usually is the `.git` Folder inside the Repo, where the Server gets the latest commit information from `refs/heads/<Branch-Name>`. To sync either you set up a git server and set the server as remote of the repository to push. Or use git webhooks to pull on pushes. 