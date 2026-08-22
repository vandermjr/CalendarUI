# CalendarUI

CalendarUI is a calendar control for Avalonia UI, developed with C# and .NET 8.

It provides a visual calendar structure for date navigation, period selection, and event presentation across different view modes.

## Status

Work in progress.

## Features

- Date navigation and selection.
- Period selection.
- Event presentation in a calendar grid.
- Multiple view modes.
- Multi-day event support.
- Visual stacking of overlapping events.
- Avalonia UI-based interface.

## Technologies

- C#
- .NET 8
- Avalonia UI
- xUnit

## Project Structure

- `src/CalendarUI.Avalonia` — CalendarUI control library.
- `src/CalendarUI.Demo` — demonstration application.
- `src/CalendarUI.Tests` — automated tests.

## Requirements

- .NET 8 SDK

## Getting Started

Restore dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

Run the tests:

```bash
dotnet test
```

Run the demonstration application:

```bash
dotnet run --project src/CalendarUI.Demo
```

## Documentation

See the [CalendarUI documentation](docs/CalendarUI/index.html) for the control's structure, concepts, view modes, and behavior.

## Development

CalendarUI is developed incrementally, with a focus on:

- preserving existing behavior;
- simple and maintainable code;
- low coupling;
- automated testing;
- small and verifiable changes.

## License

To be determined.
