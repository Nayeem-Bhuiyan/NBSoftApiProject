$modulePath = Join-Path $PSScriptRoot "feature-file.psm1"

if (Test-Path $modulePath) {
    Import-Module $modulePath -Force
    Write-Host "CQRS Module Loaded Successfully ✔" -ForegroundColor Green
}
else {
    Write-Host ("CQRS Module NOT FOUND at: {0}" -f $modulePath) -ForegroundColor Red
}
