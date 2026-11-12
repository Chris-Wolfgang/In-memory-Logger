type: internal

`dotnet pack` now runs package validation against the last published version (0.1.3), so a binary-breaking change fails the pack before it can be released.
