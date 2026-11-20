type: fix

The `net8.0` asset depends on Microsoft.Extensions.Logging and Microsoft.Extensions.Logging.Abstractions 10.0.15, so `net8.0` and `net9.0` apps no longer get the "doesn't support net8.0" build warning from the 11.0 packages; every other asset depends on 11.0.0.
