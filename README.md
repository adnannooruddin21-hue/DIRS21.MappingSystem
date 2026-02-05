# Dirs21 Mapping System

A **extensible object mapping framework** for converting reservation data between partner formats.

---

## Overview

This framework provides a flexible, extensible system for mapping reservation data between the internal Dirs21 format and various partner formats (e.g., Google, Expedia etc). It emphasizes type safety, auto-discovery, and separation of concerns.

---

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Code                          │
│                    (Demo, Tests, API)                       │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                       MapHandler                            │
│            (Orchestrates mapping operations)                │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    MapperRegistry                           │
│         (Stores & resolves mapper instances)                │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              IObjectMapper<TSource, TTarget>                │
│            (Concrete mapper implementations)                │
└─────────────────────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                  IPartnerRules<T>                           │
│      (Partner-specific validation & transformation)         │
└─────────────────────────────────────────────────────────────┘
```

---

## Key Classes & Responsibilities

### Core Framework Components

| Class | Location | Responsibility |
|-------|----------|---------------|
| **IObjectMapper<TSource, TTarget>** | `Core/IObjectMapper.cs` | Generic interface for type-safe mapping between source and target types |
| **MapperRegistry** | `Core/MapperRegistry.cs` | Stores all mappers; resolves mappers by type; auto-discovers mappers from assemblies |
| **MapHandler** | `Core/MapHandler.cs` | Entry point for mapping operations; provides high-level API (Map, MapCollection, MapAndValidate) |
| **IPartnerRules<T>** | `Core/IPartnerRules.cs` | Interface for partner-specific validation and transformation rules |
| **MappingResult<T>** | `Core/MappingResult.cs` | Contains mapping result, validation status, and error messages |

### Implementation Examples

| Component | Location | Purpose |
|-----------|----------|---------|
| **Dirs21ToGoogleReservationMapper** | `Mappers/` | Maps Dirs21 reservation → Google format |
| **GoogleToDirs21ReservationMapper** | `Mappers/` | Maps Google reservation → Dirs21 format |
| **GoogleReservationRules** | `PartnerRules/` | Google-specific validation and transformations |

---

## Project Structure

```
Dirs21.MappingSystem/
├── Dirs21.Mapping/                      # Core library
│   ├── Core/                            # Framework components
│   │   ├── IObjectMapper.cs             # Mapper interface
│   │   ├── MapperRegistry.cs            # Registry with auto-discovery
│   │   ├── MapHandler.cs                # High-level API
│   │   ├── MappingResult.cs             # Validation result wrapper
│   │   └── IPartnerRules.cs             # Partner rules interface
│   ├── Models/                          # Data models for all formats
│   │   ├── Dirs21/Reservation.cs        # Internal Dirs21 format
│   │   └── Google/Reservation.cs        # Google partner format
│   ├── Mappers/                         # Concrete mapper implementations
│   │   ├── Dirs21ToGoogleReservationMapper.cs
│   │   └── GoogleToDirs21ReservationMapper.cs
│   └── PartnerRules/                    # Partner-specific business rules
│       └── Google/                      # Google-specific rules & enums
│           ├── GoogleReservationRules.cs        # Business rule validator
│           ├── GoogleReservationErrors.cs       # Enum for validation errors
│           └── GoogleReservationErrorsExtension.cs  # Enum → message extension
├── Dirs21.Mapping.Demo/                 # Demo application
│   └── Program.cs                       # Demonstrates all features
├── Dirs21.Mapping.Tests/                # Unit tests
│   ├── GoogleMappingTests.cs
│   ├── MapHandlerTests.cs
│   ├── PartnerRulesTests.cs
│   ├── AutoRegistrationTests.cs
│   └── ExtensibilityTests.cs
└── README.md                            # Comprehensive technical documentation
```

---

## How to Add a New Partner

Adding a new partner (e.g., Expedia) requires **zero changes** to existing code.

### Step 1: Define Partner Model
Create a new folder `Models/{PartnerName}/` and define the partner's data structure.

**File:** `Models/Expedia/Reservation.cs`
- Define properties matching the partner's API/format
- Use appropriate data types

### Step 2: Create Forward Mapper (Dirs21 → Partner)
Create a mapper class implementing `IObjectMapper<TSource, TTarget>`.

**File:** `Mappers/Dirs21ToExpediaReservationMapper.cs`
- Implement `IObjectMapper<Dirs21.Reservation, Expedia.Reservation>`
- Transform Dirs21 properties to Expedia properties
- Must be `public` with parameterless constructor

### Step 3: Create Reverse Mapper (Partner → Dirs21)
Create the reverse mapper for bi-directional mapping.

**File:** `Mappers/ExpediaToDirs21ReservationMapper.cs`
- Implement `IObjectMapper<Expedia.Reservation, Dirs21.Reservation>`
- Transform Expedia properties back to Dirs21 properties

### Step 4: Create Partner Rules
Define partner-specific validation and transformation rules.

**File:** `PartnerRules/Expedia/ExpediaReservationRules.cs`
- Implement `IPartnerRules<Expedia.Reservation>`
- **Apply()**: Apply transformations (add prefixes, format data, etc.)
- **Validate()**: Validate against partner-specific requirements

### Step 5: Done! (Auto-Registration)
The framework automatically discovers and registers your mappers at startup. No manual registration needed.

**Requirements for Auto-Discovery:**
- Class must be `public`
- Class must not be `abstract`
- Class must implement `IObjectMapper<TSource, TTarget>`
- Class must have a parameterless constructor

---

## Key Features

### 1. Type-Safe Mapping
Uses generic interfaces for compile-time type checking, eliminating runtime type errors.

### 2. Auto-Discovery
Automatically scans assemblies and registers all mappers at startup using reflection.

### 3. Collection Mapping
Built-in support for mapping lists of objects efficiently.

### 4. Integrated Validation
Map and validate in a single operation with `MapAndValidate()`.

### 5. Bi-Directional Mapping
Supports both forward (Dirs21 → Partner) and reverse (Partner → Dirs21) mappings.

---

## Technical Specifications

- **Framework:** .NET 10.0
- **Language:** C# 13
- **Dependencies:** None (zero external packages)
- **Test Framework:** xUnit

---

## Running the Project

### Build
```bash
dotnet build
```

### Run Demo
```bash
dotnet run --project Dirs21.Mapping.Demo/Dirs21.Mapping.Demo.csproj
```

### Run Tests
```bash
dotnet test
```
---

## Documentation

- **README.md** (this file) - Quick overview and getting started
