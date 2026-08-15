# CalendarUI

Um controle de calendário para Avalonia UI, desenvolvido em C# e .NET 8.

## Status

Projeto em desenvolvimento.

## Tecnologias

- C#
- .NET 8
- Avalonia UI 12.1.1
- xUnit

## Estrutura

- `src/CalendarUI.Avalonia` — biblioteca do controle CalendarUI
- `src/CalendarUI.Demo` — aplicação de demonstração
- `src/CalendarUI.Tests` — testes automatizados

## Requisitos

- .NET 8 SDK

## Executar

Restaurar dependências:

```bash
dotnet restore
```

Compilar:

```bash
dotnet build
```

Executar os testes:

```bash
dotnet test
```

Executar a demonstração:

```bash
dotnet run --project src/CalendarUI.Demo
```

## Desenvolvimento

O projeto está sendo desenvolvido de forma incremental, priorizando:

- preservação do comportamento existente;
- testes automatizados;
- baixo acoplamento;
- código simples e sustentável.

## Licença

A definir.
