param(
    [string]$BUILD_ENV,
    [Parameter(Mandatory = $true)][string]$APP_VERSION)

$ErrorActionPreference = "Stop"

$platformRes = (az resource list --tag stack-name=shared-container-registry | ConvertFrom-Json)
if (!$platformRes) {
    throw "Unable to find eligible platform container registry!"
}
if ($platformRes.Length -eq 0) {
    throw "Unable to find 'ANY' eligible platform container registry!"
}

$acr = ($platformRes | Where-Object { $_.tags.'stack-environment' -eq 'prod' })
if (!$acr) {
    throw "Unable to find eligible prod container registry!"
}
$AcrName = $acr.Name

$platformRes = (az resource list --tag stack-name=shared-storage | ConvertFrom-Json)
if (!$platformRes) {
    throw "Unable to find eligible platform storage!"
}
if ($platformRes.Length -eq 0) {
    throw "Unable to find 'ANY' eligible platform storage!"
}

$str = ($platformRes | Where-Object { $_.tags.'stack-environment' -eq 'prod' })
if (!$str) {
    throw "Unable to find eligible storage account!"
}
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

# Do not change this as this affect container reg
$namePrefix = "contoso-demo"
$apps = @(
    @{
        name = "$namePrefix-member-portal";
        path = "DemoMemberPortal";
    },
    @{
        name = "$namePrefix-website";
        path = "DemoWebsite";
    },
    @{
        name = "$namePrefix-member-points-service";
        path = "DemoCustomerServicePoints";
    },    
    @{
        name = "$namePrefix-member-service";
        path = "DemoCustomerServiceMember";
    },
    @{
        name = "$namePrefix-alternate-id-service";
        path = "DemoCustomerServiceAltId";
    },
    @{
        name = "$namePrefix-partner-api";
        path = "DemoPartnerAPI";
    },
    @{
        name = "$namePrefix-service-bus-shipping-func";
        path = "DemoServiceBusShippingFunc";
    },
    @{
        name = "$namePrefix-storage-queue-func";
        path = "DemoStorageShippingFunc";
    }
)


for ($i = 0; $i -lt $apps.Length; $i++) {
    $app = $apps[$i]

    $appName = $app.name
    $path = $app.path    

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
        az acr build --image $imageName --build-arg version=$APP_VERSION.Replace("v", "") -r $AcrName --file ./$path/Dockerfile .
    
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
        
        dotnet publish -c Release -o out /p:Version=$APP_VERSION
    
        Compress-Archive out\* -DestinationPath $appFileName -Force
    
        # Seem like question mark is causing appfilename to be removed
        $url = "https://$AccountName.blob.core.windows.net/$ContainerName/" + $appFileName + "?$sas"    
        azcopy_v10 copy $appFileName $url --overwrite=false
    
        if ($LastExitCode -ne 0) {
            throw "An error has occured. Unable to deploy zip."
        }
    
        Pop-Location
    }
}

Push-Location Db
if ($BUILD_ENV -eq 'dev') {
    $dbFileName = "Migrations-$APP_VERSION-dev.sql"
    $dacpac = "cch-$APP_VERSION-dev.dacpac"
}
else {
    $dbFileName = "Migrations-$APP_VERSION.sql"
    $dacpac = "cch-$APP_VERSION.dacpac"
}

$url = "https://$AccountName.blob.core.windows.net/$ContainerName/" + $dbFileName + "?$sas"    
azcopy_v10 copy "Migrations.sql" $url --overwrite=false

$url = "https://$AccountName.blob.core.windows.net/$ContainerName/" + $dacpac + "?$sas"
azcopy_v10 copy "cch.dacpac" $url --overwrite=false
Pop-Location
