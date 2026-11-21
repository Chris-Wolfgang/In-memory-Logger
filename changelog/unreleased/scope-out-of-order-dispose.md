type: fix

A scope disposed before a scope nested inside it now leaves `Scopes` at once, and both scopes leave the stack once the inner one is disposed, instead of the outer scope staying reported for the rest of the async flow.
