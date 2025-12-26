# Geo.Smart.Template.EfCoreReverse 使用說明

* 本模組依照 [
EntityFramework-Reverse-POCO-Code-First-Generator](https://github.com/sjh37/EntityFramework-Reverse-POCO-Code-First-Generator) 將其修改為符合巨鷗開發規範的版本，並由 T4 範本的方式改為傳統的 console 專案，僅能使用 `.net Framework 4.8` 版本引用參考，但是產出的目標專案可為 dotnet core 版本

## 模組引用後，需修改項目說明

### 一、將 Templates.EFCore7 資料夾內的範本檔的屬性【複製到輸出目錄】設定為【一律複製】

### 二、修改 EfCoreGenerator.cs 的參數設定

1. 設定要產生 Code First 程式碼的根目錄路徑
1. 設定專案的 Namespace
1. 設定資料庫連線字串

```c#
public static void Generate()
{
    // 要產生 Code First 程式碼的根目錄路徑
    var root = "../../../Geo.Smart.Template.EfCoreReverse.DataAccess";
    Settings.Root = Path.Combine(root, "");

    // 設定專案的 Namespace
    var nameSpace = "Geo.Smart.Template.EfCoreReverse.DataAccess";

    // 設定資料庫連線字串
    var connectionString =
        @"Data Source=DevDb2\Gdb2017;Initial Catalog=GeoTemplateCoreApi;user id=sa;password=smart_admin;MultipleActiveResultSets=True;Application Name=Generator;";
    if (!TryConnect(connectionString))
    {
        throw new ArgumentException("資料庫連線失敗！請重新檢查。");
    }

    SetupDatabase(connectionString, nameSpace);
    Run();
}
```

### 三、加入 GdbContextPartial.cs

將檔案 `GdbContextPartial.cs.temp` 重新命名為 `GdbContextPartial.cs`，並將其加入到專案中

### 四、執行 console 專案

執行 console 專案並檢驗 Code First 程式碼是否正確產生
```csharp
private static void Main(string[] args)
{
    EfCoreGenerator.Generate();
    Console.WriteLine("done!");
}
```

### 五、其他設定

增加列舉型別資料：先增加列舉檔案於 `Enum` 資料夾內，並於 `EfCoreGenerator.cs` 中約第 123 行加入設定

```c#
Settings.AddEnumDefinitions = delegate (List<EnumDefinition> enumDefinitions)
{
    enumDefinitions.Add(new EnumDefinition
    {
        Schema = Settings.DefaultSchema,
        Table = "News",
        Column = "NewsType",
        EnumType = "NewsType"
    });
};
```

## 六、nupkg 套件編譯

```
nuget pack Geo.Smart.Template.EfCoreReverse.nuspec
```