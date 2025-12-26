# Ollama 安裝筆記

## docker 安裝

採用 Docker 容器運行Ollama，成功執行的話，Ollama 會在本機監聽 Port 11434 ，開啟瀏覽器查看 `http://localhost:11434`，如果有顯示 Ollama is running ，就代表 Ollama 已經成功啟動

```
docker run --name ollama -v ollama:/root/.ollama -p 11434:11434 ollama/ollama
```

2. 下載 Meta 的 [llama3.2:3b](https://ollama.com/library/llama3.2:3b) 模型 (也可以試試 `llama3.1:8b`) 並運行，所有已上架的模型可以在 [ollama.com](https://ollama.com/search) 找到

```
docker exec -it ollama ollama run llama3.2:3b
```

3. 再次啟動

```
docker ps -a
# 找到了 Container ID，例如：f3dac8ef2187

docker start f3dac8ef2187
docker stop f3dac8ef2187
```

## MCP Server

- MCP Tools 的參數不要設定為 nullable，Ollama 的型別推斷會有錯誤
> The JSON value could not be converted to System.String. Path: $.properties.lyrOpa.type | LineNumber: 0 | BytePositionInLine: 998.



## 參考資源

- [Working with multiple language models in Semantic Kernel](https://dev.to/stormhub/working-with-multiple-language-models-in-semantic-kernel-31gk?utm_source=chatgpt.com)
- [Using Semantic Kernel with Dependency Injection](https://devblogs.microsoft.com/semantic-kernel/using-semantic-kernel-with-dependency-injection/)

