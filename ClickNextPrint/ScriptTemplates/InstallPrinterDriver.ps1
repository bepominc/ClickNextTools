# Configuration variables. These must be set per the instructions in the local README file.
$infFileName = "{{VAR_INFFILENAME}}"
$driverName = "{{VAR_DRIVERNAME}}"
$printerDisplayName = "{{VAR_PRINTERNAME}}"
$printerAddress = "{{VAR_PRINTERADDRESS}}"
$duplexingMode = {{VAR_DUPLEXINGMODE}}
$isColor = {{VAR_COLOR}}
$isCollate = {{VAR_COLLATE}}

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

# Register the driver using the provided manifest with the print service.
try {
    $pnputilArgs = @(
        "/add-driver"
        "$PSScriptRoot\Drivers\$infFileName"
        "/install"
    )

    Write-ClickNextLog "Registering driver manifest $infFileName with pnputil"
    Start-Process pnputil.exe -ArgumentList $pnputilArgs -Wait -PassThru
    Write-ClickNextLog "Successfully registered driver manifest with pnputil"
}
catch {
    $err = $_
    Write-ClickNextLog "Failed to register driver manifest with pnputil: $err"
    Write-Error "Error during installation. Refer to $logPath for more information."
    Write-ClickNextLog "Installation failed"
    exit 1
}

# Install the registered driver.
try {
    $existingDriver = Get-PrinterDriver -Name $driverName -ErrorAction SilentlyContinue

    if (-not $existingDriver) {
        Write-ClickNextLog "Adding printer driver $driverName"
        Add-PrinterDriver -Name $driverName -Confirm:$false
        Write-ClickNextLog "Successfully added printer driver"
    }
    else {
        Write-ClickNextLog "Printer driver $driverName already exists - skipping driver installation"
    }
}
catch {
    $err = $_
    Write-ClickNextLog "Failed to add printer driver: $err"
    Write-Error "Error during installation. Refer to $logPath for more information."
    Write-ClickNextLog "Installation failed"
    exit 1
}

# Add the printer port if it doesn't already exist.
try {
    $existingPrinterPort = Get-Printerport -Name "IP_$printerAddress" -ErrorAction SilentlyContinue

    if (-not $existingPrinterPort) {
        Write-ClickNextLog "Creating printer port IP_$printerAddress"
        Add-PrinterPort -Name "IP_$printerAddress" -PrinterHostAddress "$printerAddress" -Confirm:$false
        Write-ClickNextLog "Successfully created printer port"
    }
    else {
        Write-ClickNextLog "Printer port IP_$printerAddress already exists - skipping printer port creation"
    }
}
catch {
    $err = $_
    Write-ClickNextLog "Failed to add printer port: $err"
    Write-Error "Error during installation. Refer to $logPath for more information."
    Write-ClickNextLog "Installation failed"
    exit 1
}

# Add and configure the printer.
try {
    $existingPrinter = Get-Printer -Name $printerDisplayName -ErrorAction SilentlyContinue
    if ($existingPrinter) {
        Write-ClickNextLog "Printer $printerDisplayName already exists - removing old printer"
        Remove-Printer -Name $printerDisplayName -Confirm:$false
    }

    Write-ClickNextLog "Adding printer $printerDisplayName"
    Add-Printer -Name $printerDisplayName -ShareName $printerDisplayName -PortName "IP_$printerAddress" -DriverName $driverName

    $addedPrinter = Get-Printer -Name $printerDisplayName -ErrorAction SilentlyContinue
    if (-not $addedPrinter) {
        Write-ClickNextLog "Failed to add printer $printerDisplayName - printer not found by commandlet after installation task"
        Write-Error "Error during installation. Refer to $logPath for more information."
        Write-ClickNextLog "Installation failed"
        exit 1
    }
    Write-ClickNextLog "Printer $printerDisplayName added successfully"

    Write-ClickNextLog "Setting configuration options"
    Set-PrintConfiguration -PrinterName $printerDisplayName -DuplexingMode $duplexingMode
    Set-PrintConfiguration -PrinterName $printerDisplayName -Color $isColor
    Set-PrintConfiguration -PrinterName $printerDisplayName -Collate $isCollate
    Write-ClickNextLog "The following configurations were made: DuplexingMode = $duplexingMode | Color = $isColor | Collate = $isCollate"
}
catch {
    $err = $_
    Write-ClickNextLog "Failed to add printer: $err"
    Write-Error "Error during installation. Refer to $logPath for more information."
    Write-ClickNextLog "Installation failed"
    exit 1
}
