# ServiceNowTableEnumerator

Utility for scanning tables in a ServiceNow instance for widget security misconfigurations causing dataleaks. Scanning the instance does not require an authenticated user.

## Build

You can build the solution by using the command in the .NET CLI:

```
dotnet publish -f net8.0-windows10.0.19041.0 -c Release
```
## Usage

- Start by editing the tables.cfg file to include the tables you want to scan for access.

- Provide the instance URL in the instance url field, and press scan.
  
- Press the Scan button. Each line will change color depending on the status of the scan:
  - Green: Table is not publicly accessble
  - Red: Table is publicly available
  - Black: Result did not indicate if the table is publicly available or not. If an error occurs during the scan of the table, the color of table row will stay black.
 
- A summary will be presented at the end of the scan. You can verify the amount of affected tables.

