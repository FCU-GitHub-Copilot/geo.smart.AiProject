# 本腳本為建置 SMART AI Agent Hub - DemoSC 開發端測試站
Write-Host "SMART AI Agent Hub - 【DemoSC】測試站發行..."

# 先清除 Release 資料夾
$releasePath = "Release"
if (Test-Path $releasePath) {
    Remove-Item $releasePath -Recurse -Force
}

# 1. 建置

# 前端
Set-Location ".\Geo.Smart.AiAgentHub.WebSite\"
Invoke-Expression "nvs use"
Invoke-Expression "yarn"
Invoke-Expression "yarn stag"
Copy-Item ".\dist" "..\Release\ai.geo.local\WebSite\" -Recurse -Force
Set-Location ".."

# 後端
dotnet publish ".\Geo.Smart.AiAgentHub.AiKernel\Geo.Smart.AiAgentHub.WebApi\Geo.Smart.AiAgentHub.WebApi.csproj" `
    --configuration Release `
    -o ".\Release\ai.geo.local\WebApi" `
    -r win-x64 `
    --no-self-contained

# 2. 移除 appsettings.json
$appsettingsDev = ".\Release\ai.geo.local\WebApi\appsettings.Development.json"
if (Test-Path $appsettingsDev) {
    Remove-Item $appsettingsDev -Force
}

# 3. 壓縮發行檔
& 7z a ".\Release\ai.geo.local.zip" ".\Release\ai.geo.local\*"
Write-Host "SMART AI Agent Hub - 完成 DemoSC 測試站發行...."

# 4. 部署到 ai.geo.local
Write-Host "開始部署到 DemoSC (https://ai.geo.local)...."
New-Item -Path "\\devdemosc\geo.local\ai.geo.local\WebApi\app_offline.htm" -ItemType File -Force | Out-Null
Write-Host "已建立 app_offline.htm，開始等待 10 秒...."
Start-Sleep -Seconds 10
Write-Host "等待 10 秒結束...."
robocopy ".\Release\ai.geo.local" "\\devdemosc\geo.local\ai.geo.local" /E /NFL /NDL | Out-Null
Write-Host "完成 ai.geo.local 檔案發行複製"
Remove-Item -Path "\\devdemosc\geo.local\ai.geo.local\WebApi\app_offline.htm" -Force
Write-Host "已刪除 app_offline.htm"
Write-Host "SMART AI Agent Hub - 完成部署到 ai.geo.local 測試站...."

# 5. 開啟 Release 資料夾
Start-Process ".\Release\"