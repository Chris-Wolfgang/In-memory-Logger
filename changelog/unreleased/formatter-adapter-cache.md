type: internal

`Log` reuses the formatter adapter it stores in each entry when the same formatter comes back (as it does for every `LogInformation(...)`-style call), instead of allocating one per call; `Scopes` builds its result in a single array. No behaviour change.
