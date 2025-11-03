# PayFlow - Minimal Payment Gateway (Demo)

## Objetivo
Projeto de demonstração para um gateway de pagamentos que integra múltiplos provedores (FastPay e SecurePay).
O objetivo é permitir alternância entre provedores sem alterar a lógica principal da aplicação.

## Como executar (Docker)
```bash
docker-compose up --build
```

A API ficará disponível em `http://localhost:8080`. Use o endpoint `POST /payments`.

## Exemplo de requisição
```bash
curl -X POST http://localhost:8080/payments \
  -H "Content-Type: application/json" \
  -d '{"amount": 120.50, "currency": "BRL"}'
```

## Arquitetura
- Strategy pattern via `IPaymentProvider`
- `PaymentService` escolhe provedor principal (FastPay < R$100 / SecurePay >= R$100) e tenta fallback se necessário
- Cada provedor encapsula seu próprio payload e interpretação de resposta

## Observações
- As URLs dos provedores usadas no código são fictícias (`.fake`). Para testes locais substitua por mocks ou um servidor de teste (WireMock, httpbin, etc).
