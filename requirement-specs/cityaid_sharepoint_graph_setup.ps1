<#
 CityAid — SharePoint + Microsoft Graph setup
 Creates/aligns SharePoint document libraries & metadata for CityAid,
 then demonstrates upload + metadata update + permissions via Graph.
 
 Prereqs:
 - Microsoft Graph permissions: Sites.ReadWrite.All, Files.ReadWrite.All (app or delegated)
 - (Optional) SharePoint REST for library creation if Graph template "documentLibrary" isn't used.
 - PowerShell modules: Microsoft.Graph (Install-Module Microsoft.Graph -Scope CurrentUser)
#>

param(
  [Parameter(Mandatory=$true)] [string]$TenantHost = "contoso.sharepoint.com",
  [Parameter(Mandatory=$true)] [string]$SitePath  = "sites/Pune-Alpha",   # e.g., sites/Pune-Alpha
  [Parameter(Mandatory=$true)] [string]$LibraryName = "Analysis",
  [Parameter(Mandatory=$false)] [string]$CityCode = "PUN",
  [Parameter(Mandatory=$false)] [string]$TeamCode = "AL"
)

# 1) Connect to Graph (delegated example; or use app-only with certificate)
Import-Module Microsoft.Graph -ErrorAction Stop
$scopes = @("Sites.ReadWrite.All","Files.ReadWrite.All")
Connect-MgGraph -Scopes $scopes

# 2) Resolve site by path (GET /sites/{hostname}:/{relative-path})
#    Docs: https://learn.microsoft.com/graph/api/site-getbypath
$site = Invoke-MgGraphRequest -Method GET -Uri "https://graph.microsoft.com/v1.0/sites/$TenantHost:/$SitePath"
$siteId = $site.id
Write-Host "Resolved siteId: $siteId"

# 3) Create document library (Option A: Graph 'lists' with template=documentLibrary)
#    Docs: Create list: https://learn.microsoft.com/graph/api/list-create
#    Note: In Graph, 'lists' endpoint can create document libraries by setting list.template = 'documentLibrary'.
$body = @{
  displayName = $LibraryName
  list        = @{ template = "documentLibrary" }
} | ConvertTo-Json

$lib = Invoke-MgGraphRequest -Method POST -Uri "https://graph.microsoft.com/v1.0/sites/$siteId/lists" -Body $body -ContentType "application/json" -ErrorAction SilentlyContinue
if(-not $lib){
  Write-Warning "Graph 'documentLibrary' template may not be available in your tenant/API version. Falling back to SharePoint REST with BaseTemplate 101."
  # 3b) Fallback: SharePoint REST to create Document Library (BaseTemplate = 101)
  #     Docs: https://learn.microsoft.com/sharepoint/dev/sp-add-ins/set-custom-permissions-on-a-list-by-using-the-rest-interface (REST patterns)
  #           BaseTemplate 101 references: community docs
  # NOTE: Requires an access token for SPO resource (https://$TenantHost). Provide it via $SPO_TOKEN.
  if(-not $env:SPO_TOKEN){
    throw "Set an access token for SharePoint Online in environment variable SPO_TOKEN to use REST fallback."
  }
  $spHeaders = @{
    "Authorization" = "Bearer $($env:SPO_TOKEN)"
    "Accept"        = "application/json;odata=verbose"
    "Content-Type"  = "application/json;odata=verbose"
  }
  $restUrl = "https://$TenantHost/$SitePath/_api/web/lists"
  $restBody = @{
    "__metadata"      = @{ "type" = "SP.List" }
    "Title"           = $LibraryName
    "BaseTemplate"    = 101
    "ContentTypesEnabled" = $true
    "Description"     = "CityAid $TeamCode $CityCode Analysis"
  } | ConvertTo-Json
  $lib = Invoke-RestMethod -Method POST -Uri $restUrl -Headers $spHeaders -Body $restBody
}

# Grab list & drive identifiers (Graph treats doc libraries as both list + drive)
$lists = Invoke-MgGraphRequest -Method GET -Uri "https://graph.microsoft.com/v1.0/sites/$siteId/lists?$`filter=displayName eq '$LibraryName'"
$listId  = $lists.value[0].id
$drive   = Invoke-MgGraphRequest -Method GET -Uri "https://graph.microsoft.com/v1.0/sites/$siteId/lists/$listId?$`expand=drive"
$driveId = $drive.drive.id
Write-Host "Library created. listId=$listId driveId=$driveId"

# 4) Create metadata columns aligned to FileAttach { CaseID, City, Team, Sensitivity, ApprovalState }
#    Docs: Create columnDefinition in a list: https://learn.microsoft.com/graph/api/list-post-columns
function Add-GraphColumn($listId, $name, $kind, $choices = @()){
  $col = @{
    name = $name
  }
  switch($kind){
    "text" { $col.text = @{} }
    "choice" { $col.choice = @{ allowTextEntry = $false; choices = $choices } }
  }
  Invoke-MgGraphRequest -Method POST -Uri "https://graph.microsoft.com/v1.0/sites/$siteId/lists/$listId/columns" -Body ($col | ConvertTo-Json) -ContentType "application/json"
}

Add-GraphColumn -listId $listId -name "CaseID" -kind "text"
Add-GraphColumn -listId $listId -name "City" -kind "text"
Add-GraphColumn -listId $listId -name "Team" -kind "text"
Add-GraphColumn -listId $listId -name "Sensitivity" -kind "choice" -choices @("Low","Medium","High")
Add-GraphColumn -listId $listId -name "ApprovalState" -kind "choice" -choices @("Initiated","Pending_Analysis","Pending_Finance","Pending_PMO","Approved","Rejected")

# 5) (Optional) Break role inheritance at library level and add custom permissions (SharePoint REST).
#    Docs: Set custom permissions via REST: https://learn.microsoft.com/sharepoint/dev/sp-add-ins/set-custom-permissions-on-a-list-by-using-the-rest-interface
#    WARNING: Requires SPO access token ($env:SPO_TOKEN). Replace principal IDs and roleDefIds accordingly.
if($env:SPO_TOKEN){
  $spHeaders = @{
    "Authorization" = "Bearer $($env:SPO_TOKEN)"
    "Accept"        = "application/json;odata=verbose"
    "Content-Type"  = "application/json;odata=verbose"
  }
  # Break inheritance
  $breakUrl = "https://$TenantHost/$SitePath/_api/web/lists/getByTitle('$LibraryName')/breakroleinheritance(copyRoleAssignments=false,clearSubscopes=true)"
  Invoke-RestMethod -Method POST -Uri $breakUrl -Headers $spHeaders

  # Add role assignment: example principalId and roleDefId (Edit=1073741827). Replace with your group/user ids.
  $principalId = 5
  $roleDefId   = 1073741827
  $assignUrl   = "https://$TenantHost/$SitePath/_api/web/lists/getByTitle('$LibraryName')/roleassignments/addroleassignment(principalid=$principalId,roleDefId=$roleDefId)"
  Invoke-RestMethod -Method POST -Uri $assignUrl -Headers $spHeaders
}

# 6) Example: upload a small file (<=250 MB) to the library root folder via Graph
#    Docs: PUT .../drive/root:/path:/content
$tempFile = New-TemporaryFile
Set-Content -Path $tempFile -Value "Hello from CityAid $(Get-Date -Format o)"
$uploadPath = [System.Uri]::EscapeDataString("$LibraryName/CS-2025-$($CityCode)-$($TeamCode)-001/hello.txt")
$uploaded = Invoke-MgGraphRequest -Method PUT -Uri "https://graph.microsoft.com/v1.0/sites/$siteId/drives/$driveId/root:/$uploadPath:/content" -Body ([IO.File]::ReadAllBytes($tempFile))
$driveItemId = $uploaded.id
Write-Host "Uploaded driveItemId: $driveItemId"

# 7) Update the file's list item fields to set metadata (CaseID/City/Team/Sensitivity/ApprovalState)
#    Docs: PATCH /sites/{site-id}/lists/{list-id}/items/{item-id}/fields
#          Tip: You can get listItem from driveItem via /drives/{driveId}/items/{itemId}/listItem
$listItem = Invoke-MgGraphRequest -Method GET -Uri "https://graph.microsoft.com/v1.0/drives/$driveId/items/$driveItemId/listItem"
$itemId = $listItem.id

$fields = @{
  fields = @{
    CaseID        = "CS-2025-$CityCode-$TeamCode-001"
    City          = $CityCode
    Team          = $TeamCode
    Sensitivity   = "Low"
    ApprovalState = "Initiated"
  }
} | ConvertTo-Json
Invoke-MgGraphRequest -Method PATCH -Uri "https://graph.microsoft.com/v1.0/sites/$siteId/lists/$listId/items/$itemId/fields" -Body $fields -ContentType "application/json"

# 8) (Optional) Grant file permissions via Graph invite (adds permission on driveItem)
#    Docs: POST /drives/{drive-id}/items/{item-id}/invite
$permissionBody = @{
  recipients = @(@{ email = "user@contoso.com" })
  requireSignIn = $true
  sendInvitation = $false
  roles = @("read")
} | ConvertTo-Json
Invoke-MgGraphRequest -Method POST -Uri "https://graph.microsoft.com/v1.0/drives/$driveId/items/$driveItemId/invite" -Body $permissionBody -ContentType "application/json"

Write-Host "Done. Library, columns, sample file & metadata are ready."
