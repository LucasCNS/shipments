# Exemplos de Curl - Endpoint POST /v1/shipments

Este documento contém exemplos práticos de como usar o endpoint de criação de envios usando `curl`.

---

## 🎯 Endpoint

```
POST /v1/shipments
Host: localhost:5001
```

---

## ✅ Exemplo 1: Requisição Bem-Sucedida (201 Created)

**Descrição**: Cria um envio com dados válidos.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: alice_smith" \
  -d '{
    "packageName": "Smartphone Samsung Galaxy S24",
    "weight": 0.5,
    "dimensions": {
      "length": 15,
      "width": 8,
      "height": 2
    },
    "shippingCost": 1500,
    "destinationAddress": "Av. Corrientes 1234, CABA, Argentina"
  }'
```

**Resposta Esperada (201 Created)**:
```json
{
  "id": "f1a30750-e9c8-43cb-86dd-e20b60b727e0",
  "packageName": "Smartphone Samsung Galaxy S24",
  "weight": 0.5,
  "dimensions": {
    "length": 15,
    "width": 8,
    "height": 2
  },
  "shippingCost": 1500,
  "destinationAddress": "Av. Corrientes 1234, CABA, Argentina",
  "dateCreated": "2026-02-22T14:30:45.1234567Z",
  "dateLastUpdated": "2026-02-22T14:30:45.1234567Z",
  "creator": "alice_smith",
  "status": "pending",
  "error": null
}
```

---

## ❌ Exemplo 2: PackageName Vazio (400 Bad Request)

**Descrição**: Tenta enviar sem o PackageName, o que violará a validação.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: bob_johnson" \
  -d '{
    "packageName": "",
    "weight": 10.5,
    "dimensions": {
      "length": 30,
      "width": 20,
      "height": 15
    },
    "shippingCost": 2500,
    "destinationAddress": "Rua das Flores 456, São Paulo, Brasil"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "EMPTY_PACKAGE_NAME",
  "status": 400,
  "detail": "PackageName is required and cannot be empty."
}
```

---

## ❌ Exemplo 3: PackageName com Caracteres Especiais (400 Bad Request)

**Descrição**: PackageName contém caracteres especiais não permitidos.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: charlie_brown" \
  -d '{
    "packageName": "Package@#$%Special!",
    "weight": 5.0,
    "dimensions": {
      "length": 20,
      "width": 10,
      "height": 10
    },
    "shippingCost": 1200,
    "destinationAddress": "Calle Principal 789, Madrid, España"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "INVALID_PACKAGE_NAME",
  "status": 400,
  "detail": "PackageName contains invalid characters."
}
```

---

## ❌ Exemplo 4: Weight Inválido (400 Bad Request)

**Descrição**: Weight é zero ou negativo.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: diana_prince" \
  -d '{
    "packageName": "Invalid Weight Package",
    "weight": 0,
    "dimensions": {
      "length": 20,
      "width": 10,
      "height": 10
    },
    "shippingCost": 500,
    "destinationAddress": "Piccadilly Circus, London, UK"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "INVALID_WEIGHT",
  "status": 400,
  "detail": "Weight must be greater than zero."
}
```

---

## ❌ Exemplo 5: Dimensions Inválidas (400 Bad Request)

**Descrição**: Uma das dimensões é menor ou igual a zero.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: evan_turner" \
  -d '{
    "packageName": "Box with invalid dimensions",
    "weight": 2.5,
    "dimensions": {
      "length": 0,
      "width": 10,
      "height": 10
    },
    "shippingCost": 800,
    "destinationAddress": "123 Main St, New York, USA"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "INVALID_DIMENSIONS",
  "status": 400,
  "detail": "All dimensions (length, width, height) must be greater than zero."
}
```

---

## ❌ Exemplo 6: ShippingCost Inválido (400 Bad Request)

**Descrição**: ShippingCost é negativo.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: fiona_appleby" \
  -d '{
    "packageName": "Negative Cost Package",
    "weight": 3.0,
    "dimensions": {
      "length": 20,
      "width": 15,
      "height": 10
    },
    "shippingCost": -100,
    "destinationAddress": "Baker Street 221B, London, UK"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "INVALID_SHIPPING_COST",
  "status": 400,
  "detail": "ShippingCost must be greater than zero."
}
```

---

## ❌ Exemplo 7: DestinationAddress Vazio (400 Bad Request)

**Descrição**: Endereço de destino não foi fornecido.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: george_miller" \
  -d '{
    "packageName": "No Address Package",
    "weight": 1.5,
    "dimensions": {
      "length": 10,
      "width": 10,
      "height": 10
    },
    "shippingCost": 500,
    "destinationAddress": ""
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "EMPTY_DESTINATION_ADDRESS",
  "status": 400,
  "detail": "DestinationAddress is required and cannot be empty."
}
```

---

## ❌ Exemplo 8: Header Creator Faltando (400 Bad Request)

**Descrição**: Header `Creator` não é fornecido na requisição.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -d '{
    "packageName": "Missing Creator Package",
    "weight": 2.0,
    "dimensions": {
      "length": 20,
      "width": 15,
      "height": 10
    },
    "shippingCost": 1000,
    "destinationAddress": "Champs-Élysées, Paris, France"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "Missing Creator Header",
  "status": 400,
  "detail": "The 'Creator' header is required."
}
```

---

## ❌ Exemplo 9: Creator Header Vazio (400 Bad Request)

**Descrição**: Header `Creator` está vazio.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: " \
  -d '{
    "packageName": "Empty Creator Package",
    "weight": 2.0,
    "dimensions": {
      "length": 20,
      "width": 15,
      "height": 10
    },
    "shippingCost": 1000,
    "destinationAddress": "Colosseum, Rome, Italy"
  }'
```

**Resposta Esperada (400 Bad Request)**:
```json
{
  "type": "about:blank",
  "title": "EMPTY_CREATOR",
  "status": 400,
  "detail": "Creator is required and cannot be empty."
}
```

---

## 📝 Exemplo 10: PackageName com Hyphens e Underscores (Válido)

**Descrição**: PackageName pode conter letras, números, hyphens e underscores.

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: henry_clark" \
  -d '{
    "packageName": "Premium-Box_v2.0",
    "weight": 4.5,
    "dimensions": {
      "length": 25,
      "width": 20,
      "height": 15
    },
    "shippingCost": 2000,
    "destinationAddress": "Via Roma 100, Milan, Italy"
  }'
```

**Resposta Esperada (201 Created)**:
```json
{
  "id": "a7f21b4e-8d5c-48f3-9b2a-c3e5d8f1g2h3",
  "packageName": "Premium-Box_v2.0",
  "weight": 4.5,
  "dimensions": {
    "length": 25,
    "width": 20,
    "height": 15
  },
  "shippingCost": 2000,
  "destinationAddress": "Via Roma 100, Milan, Italy",
  "dateCreated": "2026-02-22T15:45:30.5678901Z",
  "dateLastUpdated": "2026-02-22T15:45:30.5678901Z",
  "creator": "henry_clark",
  "status": "pending",
  "error": null
}
```

---

## 🔧 Dicas

### Desabilitar Verificação de Certificado SSL (apenas para desenvolvimento local)

Se você receber erro de certificado SSL ao testar com `https://localhost:5001`, adicione a flag `-k`:

```bash
curl -k -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: test_user" \
  -d '{...}'
```

### Formatar JSON na Resposta

Para melhor legibilidade, pipe para `jq`:

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: test_user" \
  -d '{...}' | jq '.'
```

### Salvar Resposta em Arquivo

```bash
curl -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: test_user" \
  -d '{...}' > response.json
```

### Incluir Headers de Resposta

```bash
curl -i -X POST https://localhost:5001/v1/shipments \
  -H "Content-Type: application/json" \
  -H "Creator: test_user" \
  -d '{...}'
```

---

## 📊 Validações Resumidas

| Campo | Regra | Exemplo Válido | Exemplo Inválido |
|-------|-------|------------------|------------------|
| `packageName` | String, sem caracteres especiais, obrigatório | "Smartphone" | "@#$Invalid" |
| `weight` | Decimal > 0 | 0.5 | 0, -5.0 |
| `dimensions.length` | Decimal > 0 | 15 | 0, -10 |
| `dimensions.width` | Decimal > 0 | 8 | 0, -5 |
| `dimensions.height` | Decimal > 0 | 2 | 0, -3 |
| `shippingCost` | Decimal > 0 | 1500 | 0, -100 |
| `destinationAddress` | String não vazia, obrigatório | "Av. Corrientes 1234, CABA" | "" |
| `Creator` (header) | String não vazio, obrigatório | "alice_smith" | "", não fornecido |

---

## 🎬 Teste Rápido (Script Bash)

```bash
#!/bin/bash

BASE_URL="https://localhost:5001/v1/shipments"
CREATOR="test_automation"

echo "=== Teste 1: Envio Válido ==="
curl -k -X POST $BASE_URL \
  -H "Content-Type: application/json" \
  -H "Creator: $CREATOR" \
  -d '{
    "packageName": "Test Package",
    "weight": 1.5,
    "dimensions": {"length": 10, "width": 10, "height": 10},
    "shippingCost": 500,
    "destinationAddress": "Test Street 123"
  }' | jq '.'

echo -e "\n=== Teste 2: Weight Inválido ==="
curl -k -X POST $BASE_URL \
  -H "Content-Type: application/json" \
  -H "Creator: $CREATOR" \
  -d '{
    "packageName": "Test Package",
    "weight": 0,
    "dimensions": {"length": 10, "width": 10, "height": 10},
    "shippingCost": 500,
    "destinationAddress": "Test Street 123"
  }' | jq '.'

echo -e "\n=== Teste 3: Creator Faltando ==="
curl -k -X POST $BASE_URL \
  -H "Content-Type: application/json" \
  -d '{
    "packageName": "Test Package",
    "weight": 1.5,
    "dimensions": {"length": 10, "width": 10, "height": 10},
    "shippingCost": 500,
    "destinationAddress": "Test Street 123"
  }' | jq '.'
```

Salve como `test-shipments.sh`, dê permissão (`chmod +x test-shipments.sh`) e execute (`./test-shipments.sh`).
