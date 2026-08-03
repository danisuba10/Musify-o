Music streaming backend.
Avaiable at https://meloptica.xyz

CRUD logs: Controller-level CRUD operations for Song, Album, and Artist are logged to a file `Logs/crud-operations.txt` under the API project root. The filename can be configured using the `Logging:CrudLogFileName` setting in configuration. Note: Read (GET) operations are intentionally NOT logged or notified; logging and notifications apply to Create/Update/Delete operations only. - Console logging: By default, logs are also written to the console for SSH/remote viewing. You can toggle console logging using the `Logging:ConsoleLoggingEnabled` configuration key (true/false). The default is `true`.

## .env configuration

This project uses a `.env` file loaded via DotNetEnv (see `Program.cs` where `Env.Load("../../.env")` is called). The `FileLogger` prefers environment variables when present. Add the following to your `.env` (or system environment variables):

```
# Name of the file under API/Logs
Logging__CrudLogFileName=crud-operations.txt

# Whether to log to console (true/false)
Logging__ConsoleLoggingEnabled=true
```

Notes:

- `Logging__CrudLogFileName` maps to `Logging:CrudLogFileName` for compatibility with configuration. If not set in environment, the app will fall back to configuration or the default `crud-operations.txt`.
- The `__` double-underscore is the convention used here to represent `:` in environment variables (also consistent with ASP.NET configuration environment mapping).
- After updating `.env`, restart the API. To use a system environment variable instead of a .env file, set the environment variables on the host or your CI environment and restart.
