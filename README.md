# 🧾 Sistema de Facturación — Metodología Ágil (UTA)

Proyecto de facturación desarrollado con arquitectura de **microservicios .NET 10** y frontend **React (Vite)**.

## Integrantes
- **Emilio Abril** — Api.Clientes · Api.Productos
- **Compañero** — Api.Ventas · Api.Gateway · UI.React

## Stack Tecnológico
- **Backend:** ASP.NET Core 10 (Microservicios)
- **Frontend:** React + Vite
- **Base de Datos:** SQL Server Express (Code First con EF Core)
- **Mensajería:** RabbitMQ
- **Gateway:** Ocelot

## Metodología
**Scrumban** con ramas Git Flow:
- `main` → producción
- `develop` → integración
- `feature/*` → desarrollo individual por integrante

## Estructura del Proyecto
```
Proyecto/
├── Api.Clientes/      # Microservicio de Clientes
├── Api.Productos/     # Microservicio de Productos
├── Api.Ventas/        # Microservicio de Ventas
├── Api.Gateway/       # API Gateway (Ocelot)
└── UI.React/          # Frontend (React + Vite)
```

## Ejecución
1. Levantar los 4 microservicios desde Visual Studio
2. `cd UI.React && npm run dev`
3. Acceder a `http://localhost:5173`
