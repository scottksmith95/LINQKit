rem https://github.com/StefH/GitHubReleaseNotes

SET version=1.3.10

GitHubReleaseNotes --output ReleaseNotes.md --skip-empty-releases --exclude-labels wontfix question invalid documentation --version %version% --token %GH_TOKEN%

GitHubReleaseNotes --output PackageReleaseNotes.txt --skip-empty-releases --exclude-labels wontfix question invalid documentation --template PackageReleaseNotes.template --version %version% --token %GH_TOKEN%