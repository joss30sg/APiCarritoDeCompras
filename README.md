# 🛒 Carrito de Compras - Shopping Cart

Aplicación full-stack de carrito de compras desarrollada con **ASP.NET Core 9** (Backend) y **Angular 20** (Frontend).

> 👨‍💼 **Este es uno de mis proyectos destacados.** [Ver mi portafolio completo →](./PORTFOLIO.md)

---

## ⚡ INICIO RÁPIDO (5 minutos)

### 🔧 Requisitos Previos
- **.NET SDK 9.0** o superior
- **Node.js 20+** y npm
- **Visual Studio Code** (recomendado)

### 📝 Paso 1: Abre 2 Terminales
1. Presiona `Ctrl + Backtick (`)` dos veces en VS Code
2. O ve a **Terminal → Nueva Terminal**

### 🖥️ Paso 2: Inicia el Backend (Terminal 1)
```bash
cd ShoppingCartApi
dotnet run
```
Espera a ver:
```
Now listening on: http://localhost:5276
```

### 🌐 Paso 3: Inicia el Frontend (Terminal 2)
```bash
cd Frontend/ShoppingCartUI
npm start
```
Espera a ver:
```
Application bundle generation complete
```

### 🌍 Paso 4: Abre tu navegador
- **Frontend:** http://localhost:4200
- **API:** http://localhost:5276/swagger

---

## 👤 CREDENCIALES DE PRUEBA

> ⚠️ **Usa EXACTAMENTE estas credenciales:**

| Campo | Valor |
|-------|-------|
| **Usuario** | `testuser` |
| **Contraseña** | `Password123!` |

---

## 📁 ESTRUCTURA DEL PROYECTO

```
Carrito de compras/
├── Backend/ShoppingCartApi/          # API REST (ASP.NET Core 9)
├── Frontend/ShoppingCartUI/          # UI (Angular 20)
├── Test/                             # Pruebas unitarias
└── README.md                         # Este archivo
```

---

## 🎯 CARACTERÍSTICAS PRINCIPALES

✅ **Autenticación JWT** - Login/Registro seguro  
✅ **Listado de Productos** - 6 productos con imágenes reales  
✅ **Carrito de Compras** - Agregar/editar/eliminar productos  
✅ **Historial de Órdenes** - Verifica tus compras anteriores  
✅ **API REST** - Documentación interactiva en Swagger  
✅ **Diseño Responsivo** - Compatible con móviles  
✅ **Testing** - Pruebas unitarias incluidas  

---

## 📸 PRIMEROS PASOS

### 1️⃣ Login/Registro
Abre http://localhost:4200 e iniciar sesión con:
- **Usuario:** testuser
- **Contraseña:** Password123!

### 2️⃣ Ver Productos
Verás 6 productos con fotos reales (Laptop, iPhone, AirPods, iPad, Apple Watch, Magic Keyboard)

### 3️⃣ Agregar al Carrito
Haz clic en **"Agregar al Carrito"** en cualquier producto

### 4️⃣ Ver tu Carrito
Haz clic en el ícono 🛒 en la parte superior derecha

### 5️⃣ Ver Historial
Haz clic en **"Mis Órdenes"** desde el carrito

---

## 🧪 TESTING

```bash
# Ejecutar todas las pruebas
dotnet test Test/ShoppingCartApi.Tests.csproj

# Con reporte de cobertura
dotnet test Test/ShoppingCartApi.Tests.csproj \
  --collect "XPlat Code Coverage"

# Generar HTML de cobertura
reportgenerator -reports:Test/TestResults/*/coverage.cobertura.xml \
  -targetdir:coverage-report -reporttypes:Html
```

---

## 🐛 TROUBLESHOOTING

### ❌ "Puerto 5276 ya está en uso"
```bash
# Mata el proceso anterior
taskkill /F /IM dotnet.exe

# O usa otro puerto
cd ShoppingCartApi
dotnet run --urls "http://localhost:5300"
```

### ❌ "npm start no funciona"
```bash
cd Frontend/ShoppingCartUI
npm install
npm start
```

### ❌ "No puedo acceder a API"
- Verifica http://localhost:5276/swagger
- Backend debe estar ejecutándose
- Revisa Terminal 1

---

## 📚 URLS IMPORTANTES

| Recurso | URL |
|---------|-----|
| **Frontend** | http://localhost:4200 |
| **API** | http://localhost:5276 |
| **Swagger** | http://localhost:5276/swagger |
| **Historial** | http://localhost:4200/order-history |

---

## 🚀 MÁS INFORMACIÓN

- [📋 PORTFOLIO.md](./PORTFOLIO.md) - **Ver mi portafolio completo con demos de proyectos**
- [CAMBIOS_REALIZADOS.md](./CAMBIOS_REALIZADOS.md) - Cambios implementados
- [RESUMEN_EJECUTIVO.md](./RESUMEN_EJECUTIVO.md) - Resumen del proyecto
- [COMO_VERIFICAR_COMPRAS.md](./COMO_VERIFICAR_COMPRAS.md) - Guía de compras

### Tecnologías Utilizadas
- **Backend:** ASP.NET Core 9, C#, JWT, EntityFramework
- **Frontend:** Angular 20, TypeScript, RxJS
- **Base de datos:** In-Memory (desarrollo)
- **Testing:** xUnit, Moq, FluentAssertions

---

## 📄 Licencia
MIT

## 👤 Autor
José García - Aplicación educativa con Clean Architecture
