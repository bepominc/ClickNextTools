# Configuration variables. These must be set per the instructions in the local README file.
$printerDisplayName = "{{VAR_PRINTERNAME}}"

# --- Script start. You shouldn't need to change anything below this line during configuration. ---

$logPath = "$env:TEMP\ClickNext\logs\$printerDisplayName.log"
function Write-ClickNextLog {
    param ([string]$message)
    if (!(Test-Path $logPath)) {
        New-Item -Path $logPath -ItemType File -Force
    }
    $timeStamp = Get-Date -UFormat "%Y-%m-%d %T"
    $logLine = "[$timeStamp] $message"
    Write-Host $logLine
    $logLine | Out-File -FilePath $logPath -Append
}

try {
    # Remove the printer.
    $installedPrinter = Get-Printer -Name $printerDisplayName -ErrorAction SilentlyContinue
    if (-not $installedPrinter) {
        Write-ClickNextLog "Attempted to uninstall missing printer $printerDisplayName"
        exit 0
    }
    $printerPortName = $installedPrinter.PortName
    Write-ClickNextLog "Removing printer $printerDisplayName"
    Remove-Printer -Name $printerDisplayName -Confirm:$false
    Write-ClickNextLog "Printer removed successfully"

    # Remove the printer port.
    Write-ClickNextLog "Removing printer port $printerPortName"
    Remove-PrinterPort -Name $printerPortName -Confirm:$false
    Write-ClickNextLog "Printer port removed successfully"
}
catch {
    $err = $_
    Write-ClickNextLog "Failed to remove printer: $err"
    Write-Error "Error during uninstallation. Refer to $logPath for more information."
    Write-ClickNextLog "Uninstall failed"
    exit 1
}
