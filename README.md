# ClickNextTools

Tools that make IT tasks as easy as clicking "Next"!

## ClickNextPrint

Easily generate Intune application bundles for IPv4 enabled printers.

### Using The Output
In the following section replace the following values with your own:
- Replace `<ScriptName>` with the name selected when exporting. Default is the printer name with any spaces removed. When in doubt, adding the bundle to a new app in Intune will set the name to this.
- Replace `<PrinterName>` with the printer name set in the app.

Commands:
- Install: `%SystemRoot%\sysnative\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass ./<BundleName>.ps1`
- Uninstall: `%SystemRoot%\sysnative\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass ./Uninstall.ps1`

Registry Detection:
- Key path: `HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Print\Printers\<PrinterName>`
- Detection method: `String comparison`
- Value name: `Name`
- Operator: `Equals`
- Value: `<PrinterName>`
