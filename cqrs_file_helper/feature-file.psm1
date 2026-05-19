# =========================================================
# SmartApp CQRS Generator Module
# =========================================================

function Get-PluralName {
    param([string]$Name)

    $Name = $Name.Trim().ToLower()

    $irregulars = @{
        "person"   = "People"
        "child"    = "Children"
        "man"      = "Men"
        "woman"    = "Women"
        "country"  = "Countries"
        "company"  = "Companies"
        "category" = "Categories"
        "genre"    = "Genres"
        "gender"   = "Genders"
    }

    if ($irregulars.ContainsKey($Name)) {
        return $irregulars[$Name]
    }

    if ($Name.EndsWith("y")) {
        return ($Name.Substring(0, $Name.Length - 1) + "ies")
    }
    elseif ($Name.EndsWith("s")) {
        return $Name
    }
    else {
        return ($Name + "s")
    }
}

function New-CqrsFeature {

    param(
        [Parameter(Mandatory = $true)]
        [string]$FeatureName,

        [string]$Module = "MasterData"
    )

    # Normalize
    $FeatureName = (Get-Culture).TextInfo.ToTitleCase($FeatureName.ToLower())
    $pluralName = Get-PluralName $FeatureName

    $basePath = "SmartApp.Application\Features\$Module\$pluralName"

    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "CQRS Generator Running..." -ForegroundColor Cyan
    Write-Host "Feature : $FeatureName" -ForegroundColor Cyan
    Write-Host "Folder  : $pluralName" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan

    # Folders
    $folders = @(
        "Commands\Create",
        "Commands\Update",
        "Commands\Delete",
        "Queries\GetById",
        "Queries\GetList",
        "Queries\GetPaged",
        "DTOs",
        "Mappings"
    )

    foreach ($folder in $folders) {
        New-Item -ItemType Directory -Force -Path (Join-Path $basePath $folder) | Out-Null
    }

    # Commands
    $commandTypes = @("Create", "Update", "Delete")

    foreach ($type in $commandTypes) {

        $entity = "$type$FeatureName"
        $path = Join-Path $basePath "Commands\$type"

        New-Item "$path\$entity`Command.cs" -Force | Out-Null
        New-Item "$path\$entity`Handler.cs" -Force | Out-Null
        New-Item "$path\$entity`Validator.cs" -Force | Out-Null
        New-Item "$path\$entity`Response.cs" -Force | Out-Null
    }

    # Queries
    $queries = @(
        "Get${FeatureName}ById",
        "Get${pluralName}List",
        "Get${pluralName}Paged"
    )

    foreach ($query in $queries) {

        $path = Join-Path $basePath "Queries\$query"

        New-Item "$path\$query`Query.cs" -Force | Out-Null
        New-Item "$path\$query`Handler.cs" -Force | Out-Null
        New-Item "$path\$query`Response.cs" -Force | Out-Null
    }

    Write-Host "========================================" -ForegroundColor Green
    Write-Host "CQRS Feature Created Successfully" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
}