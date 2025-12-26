# 使用 Trivy 產製 SBOM (軟體物料清單)


## Trivy 安裝

- 方式一：由【指令式軟體安裝服務】工具安裝
```
# Chocolatey 安裝
choco install trivy -y

# winget 安裝
winget install trivy

# 查看安裝版本
trivy -v
```

- 方式二：參照官方說明步驟安裝

  1. 從 [GitHub Release](https://github.com/aquasecurity/trivy/releases/) 下載 `trivy_x.xx.x_windows-64bit.zip` 檔案
  2. 解壓縮至任意資料夾位置，例如 `D:\Trivy`
  3. 將 D:\Trivy 路徑加入環境數


## Trivy 檢測

```
# trivy filesystem 掃描的語法，可用 fs 替代 fs
trivy filesystem (REPO_PATH | REPO_URL)

# 最簡單的作法，根據預設會掃描 vulnerability 跟 secret
trivy fs ./

# 也可以指定僅掃描單一檔案
trivy fs ./trivy-ci-test/Pipfile.lock

# 將結果輸出到文件， --output 可用 -o 替代
trivy fs --output ./sbom/trivy.txt ./

# 掃描設定錯誤，這時僅掃描 misconfig，預設的 vulnerability 跟 secret 要額外指定
trivy fs --scanners misconfig ./

# 四種都掃
trivy fs --scanners vuln,secret,misconfig,license --output ./sbom/trivy-all.txt ./

```

## Trivy 產製 SBOM 報告

trivy 的掃描結果，預設會直接打印在終端機上，也可以透過 --output 指定儲存到檔案中，沒有設定的話預設是產生 trivy 格式的報告，trivy 也支援產製 SBOM 格式的檔案，包括：[CycloneDX](https://trivy.dev/latest/docs/supply-chain/sbom/#cyclonedx)、[SPDX](https://trivy.dev/latest/docs/supply-chain/sbom/#spdx) 兩種格式，透過 --format 指定

```
# 輸出 CycloneDX 格式，為了使工具辨識該檔案為 CycloneDX SBOM，副檔名最好為 *.cdx.json
trivy fs --format cyclonedx --output ./sbom/trivy.cdx.json ./path/to/repo

# 輸出 SPDX 格式，為了使工具辨識該檔案為 SPDX SBOM，副檔名最好為 *.spdx.json
trivy fs --format spdx-json --output ./sbom/trivy.spdx.json ./path/to/repo
```

## 臺南園區使用 Trivy 語法

### 檢測整個原始碼

此方式會掃描全部的原始碼，檔案較大且多重複

```
# 產生 SBOM JSON 檔
trivy fs --format cyclonedx --output ./src/Geo.Smart.Tainan.Industrial/sbom/sbom-src-all.cdx.json ./src/Geo.Smart.Tainan.Industrial
```

### 產製 HTML 報告

在預設情況下，Trivy 產製的報告是自行繪製的，優點是不用依賴其他元件，但是容易跑版，其實 Trivy 也可以產製 HTML 格式的報告，首先可以到 [這裡](https://github.com/aquasecurity/trivy/blob/main/contrib/html.tpl) 下載範本檔，然後在掃描時，指定 `輸出格式(--format)`以及 `範本檔位置(--template)` 即可
> 注意：範本檔路徑最前面要有 `@` 符號

```
trivy fs --format template --template "@./trivy-html.tpl" --output "./sbom/trivy-report.html" ./
```


### 個別專案檢測

這時會分為前後端的掃描方式，掃描的檔案並不相同，因為臺南園區的專案很多，所以這裡以前台的前、後端兩個專案為例

#### 後端 dotnet

- 掃描的檔案為 `*.deps.json`，此檔案會在專案建置時產生

D:\GeoGitLab\Smart\geo.smart.tainan.industrial\src\Geo.Smart.Tainan.Industrial\Geo.Smart.Tainan.Industrial.Portal.WebApi
```
trivy fs --format cyclonedx --output ./src/Geo.Smart.Tainan.Industrial/sbom/sbom-dotnet-api.cdx.json ./src/Geo.Smart.Tainan.Industrial/Geo.Smart.Tainan.Industrial.Portal.WebApi/bin/Debug/net8.0
```

#### 前端 node.json

- 掃描的檔案為 `yarn.lock' 或 `package-lock.json` 等檔案，視專案開發設定而定

```
trivy fs --format cyclonedx --output ./src/Geo.Smart.Tainan.Industrial/sbom/sbom-yarn.cdx.json ./src/Geo.Smart.Tainan.Industrial/Geo.Smart.Tainan.Industrial.Portal.WebSite
```

> 如果是業主在索要 SBOM 軟體物料清單的話，可以先給整個專案的試試
