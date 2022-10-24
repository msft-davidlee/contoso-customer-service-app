param(
    [string]$BUILD_ENV,
    [string]$APP_PATH,
    [Parameter(Mandatory = $true)][string]$APP_VERSION)

$ErrorActionPreference = "Stop"

$acr = (az resource list --tag ard-resource-id=shared-container-registry | ConvertFrom-Json)
$AcrName = $acr.Name

$str = (az resource list --tag ard-resource-id=shared-storage | ConvertFrom-Json)
$AccountName = $str.Name
$ContainerName = "apps"

# Generate SAS upfront
$AccountKey = (az storage account keys list -g $str.ResourceGroup -n $AccountName | ConvertFrom-Json)[0].value
$end = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
$start = (Get-Date).ToString("yyyy-MM-dd")
$sas = (az storage container generate-sas -n $ContainerName --account-name $AccountName --account-key $AccountKey --permissions racwl --expiry $end --start $start --https-only | ConvertFrom-Json)
if (!$sas -or $LastExitCode -ne 0) {
    throw "An error has occured. Unable to generate sas."
}

# Login to ACR
az acr login --name $AcrName
if ($LastExitCode -ne 0) {
    throw "An error has occured. Unable to login to acr."
}

$app = Get-Content -Path $APP_PATH | ConvertFrom-Json

$appName = $app.name
$path = $APP_PATH 

$imageName = "$appName`:$APP_VERSION"

if ($BUILD_ENV -eq 'dev') {
    $imageName = "$imageName-$BUILD_ENV"
}    

$shouldBuild = $true
$tags = az acr repository show-tags --name $AcrName --repository $appName | ConvertFrom-Json
if ($tags) {
    if ($tags.Contains($APP_VERSION)) {
        $shouldBuild = $false
    }
}

if ($shouldBuild -eq $true) {
    # Build your app with ACR build command
    $ver = $APP_VERSION.Replace("v", "")
    az acr build --image $imageName --build-arg version=$ver -r $AcrName --file ./$path/Dockerfile .

    if ($LastExitCode -ne 0) {
        throw "An error has occured. Unable to build image."
    }

    Push-Location $path

    if ($BUILD_ENV -eq 'dev') {
        $appFileName = ("$appName-$APP_VERSION-dev" + ".zip")
    }
    else {
        $appFileName = ("$appName-$APP_VERSION" + ".zip")
    }
    
    dotnet publish -c Release -o out /p:Version=$ver

    Compress-Archive out\* -DestinationPath $appFileName -Force

    # Seem like question mark is causing appfilename to be removed
    $url = "https://$AccountName.blob.core.windows.net/$ContainerName/" + $appFileName + "?$sas"    
    azcopy_v10 copy $appFileName $url --overwrite=false

    if ($LastExitCode -ne 0) {
        throw "An error has occured. Unable to deploy zip."
    }

    Pop-Location
}