# TradeRisk API (.NET 10)

API REST para classificação de risco de trades e análise de distribuição de risco.

1 - Fazer o clone do repositório

2 - Definir UBSTradeRisk.Web como projeto de inicialização

3 - Compilar e Rodar 

## Requisitos
- .NET SDK 10.0
- Windows, macOS ou Linux

## Como executar via terminal
```bash
# Restaurar e build
dotnet restore
dotnet build

# Executar API
dotnet run --project src/UBSTradeRisk.Web

Acesse Swagger: http://localhost:5000/swagger ou https://localhost:5001/swagger (ou porta mostrada no console)
