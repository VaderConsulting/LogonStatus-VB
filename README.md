# LogonStatus

VB.NET WinForms tool that reports Active Directory logon and logoff status for a chosen user across one or more servers. clsUser holds last-logon / last-logoff timestamps and the servers that recorded them; frmMain drives threaded DirectoryServices lookups against the configured server list. Built for administrators investigating where an account last signed on or off.

**Source last updated:** 2012-07-07

| Field | Value |
| --- | --- |
| Language | VB.NET |
| Target | .NET Framework 2.0 |
| Output | WinForms executable |

## Solution structure

| Project | Language | Type | Purpose |
| --- | --- | --- | --- |
| `LogonStatus` | VB.NET | WinForms exe | Per-user logon/logoff status across servers |

## How to open

Open the `LogonStatus.vbproj` (or solution if present) in Visual Studio.

## Requirements

- Visual Studio 2010 or later
- .NET Framework 2.0
- Network access to the target servers for DirectoryServices queries

## Attribution and provenance

Working copy from my Historical Dev folder. No third-party source attribution markers were identified during this review.

## License

MIT. See `LICENSE`.
