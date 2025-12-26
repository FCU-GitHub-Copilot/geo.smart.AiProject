dotnet sonarscanner begin /k:"SMART-AI-Agent-Hub" /d:sonar.host.url="https://sonar.geo.local"  /d:sonar.login="sqp_5e8ec6a718346e4e631631c3135d3150e7535602"
dotnet build
dotnet sonarscanner end /d:sonar.login="sqp_5e8ec6a718346e4e631631c3135d3150e7535602"
