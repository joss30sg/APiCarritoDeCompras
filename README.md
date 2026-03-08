# Shopping Cart - Carrito de Compras 🛒

Aplicación full-stack de carrito de compras desarrollada con **ASP.NET Core 9** (Backend) y **Angular 20** (Frontend), con autenticación JWT, validación de datos y documentación Swagger.

---

## ⚡ INICIO RÁPIDO (5 minutos)

Si solo quieres ejecutar la app rápidamente, sigue estos pasos:

### Paso 1: Abre 2 Terminales en VS Code

1. En VS Code, presiona `Ctrl + Backtick (`)` dos veces para abrir 2 terminales
2. O ve a **Terminal → Nueva Terminal**

### Paso 2: Ejecuta el Backend (Terminal 1)

```bash
cd ShoppingCartApi
dotnet run
```

Espera hasta ver:
```
Now listening on: http://localhost:5276
```

✅ **Backend listo**

### Paso 3: Ejecuta el Frontend (Terminal 2)

```bash
cd ShoppingCartUI
npm install
npm start
```

Espera hasta ver:
```
Application bundle generation complete
```

✅ **Frontend listo**

### Paso 4: Abre en tu navegador

Abre: **http://localhost:5276**

👤 **Usa estas credenciales de prueba:**
- Usuario: `testuser`
- Contraseña: `Password123!`

¡Listo! Ya puedes ver productos y agregar al carrito 🎉

---

## 🚀 Requisitos Previos

- **.NET SDK 9.0** o superior
- **Node.js 20+** y npm
- **Visual Studio Code** (recomendado)
- Extensiones recomendadas:
  - REST Client (para probar `.http` files)
  - Angular Language Service

## 🛠️ Configuración Detallada

### 1️⃣ Backend - API .NET Core

#### Opción A: Línea de comandos (recomendado)

```bash
# 1. Ve al directorio del backend
cd ShoppingCartApi

# 2. (Solo primera vez) Instala dependencias
dotnet restore

# 3. Ejecuta la aplicación
dotnet run
```

#### Opción B: Con Visual Studio Code

1. Ve a la carpeta `ShoppingCartApi`
2. Abre `Program.cs`
3. Presiona `F5` para ejecutar con debug

**Evidencia de éxito:**
```
Now listening on: http://localhost:5276
Now listening on: https://localhost:7276
```

✅ Accede a la documentación API en: **http://localhost:5276/swagger**

---

### 2️⃣ Frontend - Angular

#### Opción A: Línea de comandos (recomendado)

```bash
# 1. Ve al directorio del frontend
cd ShoppingCartUI

# 2. (Solo primera vez) Instala dependencias
npm install

# 3. Ejecuta en modo desarrollo
npm start
```

**Evidencia de éxito:**
```
✔ Compiled successfully
✔ Application bundle generation complete
```

✅ Frontend disponible en: **http://localhost:4200**

#### Opción B: Instalar dependencias por primera vez

Si es tu primer ejecutable, npm necesita descargar paquetes (espera 2-5 minutos):

```bash
cd ShoppingCartUI
npm install    # ⏳ Espera aquí mientras descarga paquetes
npm start      # Luego ejecuta
```

---

## 🎯 Resumen de URLs - Acceso Rápido

| Componente | URL | Qué es |
|-----------|-----|--------|
| **Frontend** | http://localhost:4200 | App principal - Aquí ves productos |
| **API REST** | http://localhost:5276 | Servidor backend - Procesa datos |
| **Documentación API** | http://localhost:5276/swagger | Prueba endpoints sin código |

---

## �️ Cómo Agregar tus Propios Productos

¿Quieres cambiar los productos que se muestran? Sigue estos pasos:

### Paso 1: Ve al archivo de productos

1. En VS Code, abre la carpeta: `ShoppingCartApi`
2. Ve a: **Infrastructure → Repositories**
3. Abre: `InMemoryProductRepository.cs`

### Paso 2: Encuentra el código de productos

Busca esta sección (Ctrl+F):
```
public InMemoryProductRepository()
```

Verás algo como esto:
```csharp
public InMemoryProductRepository()
{
    _products.Add(1, new Product { Id = 1, Name = "Laptop", ... });
    _products.Add(2, new Product { Id = 2, Name = "Mouse", ... });
}
```

### Paso 3: Edita los productos

**Reemplaza** los productos antiguos con los tuyos. Ejemplo:

```csharp
public InMemoryProductRepository()
{
    // Producto 1: Tu primer producto
    _products.Add(1, new Product { 
        Id = 1, 
        Name = "iPhone 15 Pro", 
        Price = 999.99m, 
        ImageUrl = "https://images.unsplash.com/photo-1592286927505-1def25115558?w=400" 
    });
    
    // Producto 2: Tu segundo producto
    _products.Add(2, new Product { 
        Id = 2, 
        Name = "iPad Pro 12.9", 
        Price = 1299.99m, 
        ImageUrl = "https://images.unsplash.com/photo-1587372247318-f18282c81a98?w=400" 
    });
}
```

### Paso 4: Estructura de un Producto

Cada producto tiene 4 partes obligatorias:

```csharp
_products.Add(
    1,                                          // ID único (1, 2, 3...)
    new Product { 
        Id = 1,                                 // Mismo ID que arriba
        Name = "Nombre del Producto",           // Nombre visible en app
        Price = 99.99m,                         // Precio (número con "m" al final)
        ImageUrl = "https://ejemplo.com/img.jpg"  // URL de imagen
    }
);
```

### Paso 5: Dónde obtener imagenes gratis de calidad

Usa cualquier URL de imagen. Recomendamos:

| Sitio | Ejemplo URL |
|-------|------------|
| **Unsplash** (Recomendado) | `https://images.unsplash.com/photo-XXX?w=400` |
| **Pixabay** | `https://pixabay.com/get/g...` |
| **Pexels** | `https://images.pexels.com/photos/XXX/` |
| **Tu servidor** | `https://tudominio.com/images/foto.jpg` |

Obtén URLs gratis en: https://unsplash.com/

### Paso 6: Guarda y reinicia

1. Presiona **Ctrl+S** para guardar
2. Presiona **Ctrl+C** en Terminal 1 (Backend) para detener
3. Ejecuta nuevamente:
   ```bash
   cd ShoppingCartApi
   dotnet run
   ```
4. Espera hasta ver: `Now listening on: http://localhost:5276`
5. En el navegador, presiona **Ctrl+F5** para refrescar

¡Listo! Los nuevos productos aparecerán en la app 🎉

---

## 🔐 Credenciales de Prueba

### ✅ Usuario Listo para Usar (Recomendado)

> 📌 **Este usuario ya existe en el sistema. Copia y pega exactamente:**

| Campo | Valor |
|-------|-------|
| **Usuario** | `testuser` |
| **Contraseña** | `Password123!` |

O de forma simple:
```
Usuario:     testuser
Contraseña:  Password123!
```

**Para acceder:**
1. Abre http://localhost:5276
2. Inicia sesión con las credenciales anteriores
3. ¡Listo! Ya puedes usar el carrito de compras

---

### 🆕 Opción 2: Crear tu Propia Cuenta

Si prefieres crear una cuenta nueva:

1. En la pantalla de login, presiona **"Registrarse"** o **"Sign Up"**
2. Llena el formulario:
   - **Username:** Elige un nombre (ejemplo: `miusuario`)
   - **Password:** Crea una contraseña fuerte (ejemplo: `MiPassword123!`)
3. Presiona **"Registrarse"**
4. Inicia sesión con tu nueva cuenta

---

## 📋 Estructura del Proyecto

```
Carrito de compras con NET Core y angular/
├── ShoppingCartApi/                 # Backend - ASP.NET Core
│   ├── Domain/                      # Entidades y atributos
│   ├── Application/                 # Casos de uso (MediatR)
│   ├── Infrastructure/              # Repositorios
│   ├── Presentation/                # Controladores
│   ├── Middleware/                  # Manejo de excepciones
│   └── ShoppingCartApi.csproj
├── ShoppingCartUI/                  # Frontend - Angular
│   ├── src/app/
│   │   ├── domain/                  # Lógica empresarial
│   │   ├── application/             # Servicios y casos de uso
│   │   ├── infrastructure/          # Interceptores, guards
│   │   ├── presentation/            # Componentes y páginas
│   │   └── shared/                  # Modelos comunes
│   └── package.json
└── ShoppingCartApi.Tests/           # Pruebas unitarias
```

## 🌟 Características

### Backend
- ✅ **Autenticación JWT** con login/registro
- ✅ **Validación de datos** en DTOs
- ✅ **Gestion de Carrito** con atributos de productos
- ✅ **Swagger/OpenAPI** - Documentación interactiva
- ✅ **CORS** - Configurado para múltiples orígenes
- ✅ **Manejo de excepciones** global
- ✅ **Seguridad OWASP** - Encabezados HTTP seguros
- ✅ **Pruebas unitarias** con cobertura

### Frontend
- ✅ **Login/Register** con validaciones reactivas
- ✅ **Carrito de compras** en tiempo real
- ✅ **Listado de productos** dinámico
- ✅ **Guards de rutas** - Protección de acceso
- ✅ **Interceptor JWT** - Inyección automática de tokens
- ✅ **Diseño responsive** - Mobile first
- ✅ **Header dinámico** - Muestra usuario autenticado
- ✅ **Componentes standalone** de Angular

## 📚 API Endpoints

### Autenticación
```http
# Registro
POST /api/Auth/register
Content-Type: application/json

{
  "username": "nuevouser",
  "password": "Password123!"
}

# Login
POST /api/Auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Password123!"
}
```

### Carrito de Compras
```http
# Obtener carrito
GET /api/ShoppingCart/get-cart
Authorization: Bearer {token}

# Agregar producto
POST /api/ShoppingCart/add-product
Authorization: Bearer {token}

# Actualizar cantidad
PUT /api/ShoppingCart/update-quantity
Authorization: Bearer {token}

# Eliminar producto
DELETE /api/ShoppingCart/remove-product/{productId}
Authorization: Bearer {token}
```

## 🧪 Testing

### Cargar datos de prueba
El backend carga automáticamente 6 productos de prueba con imágenes reales y precios en soles (S/):
1. Laptop Dell XPS 13 - S/ 4,809.96
2. iPhone 15 Pro - S/ 3,699.96
3. AirPods Pro - S/ 924.96
4. iPad Air - S/ 2,219.96
5. Apple Watch Series 9 - S/ 1,479.96
6. Magic Keyboard - S/ 369.96

### Con REST Client (VS Code)
1. Abre `ShoppingCartApi/ShoppingCartApi.http`
2. Usa los botones "Send Request" en cada endpoint
3. Los tokens se guardan automáticamente

## 📱 Flujo Completo de Uso de la App

Esto es lo que puedes hacer en la aplicación:

### 1. Abre http://localhost:4200

Verás la pantalla de login o el formulario de registro.

### 2. Inicia sesión

Usa estas credenciales:
- **Usuario:** `testuser`
- **Contraseña:** `Password123!`

O crea una cuenta nueva si quieres.

### 3. Ve todos los productos

Después de login, verás una página con todos los productos disponibles con:
- 🖼️ Imagen del producto
- 📦 Nombre
- 💰 Precio
- 🛒 Botón "Agregar al Carrito"

### 4. Agrega productos al carrito

Presiona el botón "Agregar al Carrito" de cualquier producto.

Verás un cuadro de diálogo donde puedes:
- ✅ Seleccionar atributos (talle, color, etc.) si existen
- ✅ Elegir la cantidad
- ✅ Confirmar

### 5. Ve tu carrito

En la parte superior derecha hay un ícono 🛒 con el número de productos.

Presiona para ver:
- ✅ Todos tus productos
- ✅ Precio total
- ✅ Botones para aumentar/disminuir cantidad
- ✅ Botones para eliminar productos

### 6. Actualiza cantidades o elimina productos

En el carrito puedes:
- ➕ Aumentar cantidad
- ➖ Disminuir cantidad
- ❌ Eliminar producto

El total se actualiza automáticamente.

### 7. Cierra sesión

En la parte superior derecha, en el header, hay un botón para cerrar sesión.

---

## ✅ Verificación de Compras - Prevención de Compras Duplicadas

### ¿Cómo funciona el sistema de compra?

Este sistema está diseñado para **evitar que compres el mismo producto dos veces**. Una vez que compras un producto, ese producto queda registrado en tu historial de órdenes y **no podrás volver a comprarlo**.

### 📋 Paso a Paso: Verificar Compras

#### **Paso 1: Accede al Historial de Órdenes**

Hay dos formas de acceder a tu historial:

**Opción A - Desde el Carrito:**
1. Ve a tu 🛒 Carrito (esquina superior derecha)
2. Presiona el botón 📋 **"Mis Órdenes"**
3. Serás redirigido a la página de historial

**Opción B - Desde la URL:**
```
http://localhost:4200/order-history
```

#### **Paso 2: Mira tu Historial de Compras**

En la página de historial verás:
- 📊 **Estadísticas totales:** Número de órdenes y dinero gastado
- 📦 **Todas tus órdenes:** Listadas por fecha más reciente primero
- 💰 **Detalles de cada orden:** Productos comprados, cantidades y precios

#### **Paso 3: Verifica Productos Comprados**

Para **saber si ya compraste un producto específico**:

1. En el historial, expande cualquier orden para ver sus detalles
2. Busca el nombre del producto en la lista de productos comprados
3. Si lo encuentras, significa que **YA FUE COMPRADO** ✓

### ⚠️ ¿Qué pasa si intentas comprar un producto que ya compraste?

Si intentas agregar un producto al carrito que **ya está en tu historial de compras**, cuando hagas **checkout (pagar)** verás un modal rojo con advertencia:

```
⚠️ Productos Ya Comprados
Los siguientes productos ya están en tu historial de compras.
No puedes comprar el mismo producto dos veces.
```

El modal mostrará:
- 🔄 Lista de productos que ya compraste
- ✓ Indicador de "Comprado"
- Dos opciones:
  - **♻️ Eliminar del Carrito** - Remueve automáticamente esos productos
  - **📝 Revisar Carrito** - Vuelve al carrito para que hagas cambios

### ✅ Casos Permitidos

Sí puedes hacer esto:
- ✅ Comprar **DIFERENTES** productos múltiples veces
- ✅ Agregar el **MISMO PRODUCTO** varias veces en una sola compra (cantidades)
- ✅ Comprar productos nuevos que no están en tu historial

### ❌ Casos NO Permitidos

No puedes hacer esto:
- ❌ Comprar un producto que ya está en tu historial de órdenes
- ❌ El carrito rechazará la compra automáticamente

### 💡 Ejemplo Práctico

**Escenario:**
- Primer día: Compras "iPhone 15 Pro" y "AirPods Pro"
- Segundo día: Intentas comprar "iPhone 15 Pro" de nuevo

**Lo que sucede:**
1. Aún puedes agregar "iPhone 15 Pro" al carrito
2. Pero cuando hagas click en **Pagar/Checkout**
3. Verás el modal de advertencia
4. Debes eliminar ese producto del carrito para poder pagar

---

## 🌟 Características de la Aplicación

### JWT Tokens
- Tokens almacenados en `localStorage`
- Inyectados automáticamente en headers Authorization
- Validados en cada petición al backend
- Redirigen a login si expiran (401)

### Guards de Rutas
- ✅ `authGuard` - Requiere autenticación para acceder
- ✅ `noAuthGuard` - Redirige a home si ya está autenticado

### CORS
Configurado para permitir:
- `http://localhost:4200` (Angular dev)
- `http://localhost:5276` (API local)
- Métodos: GET, POST, PUT, DELETE
- Headers custom y autorización

## 🆘 Solución de Problemas

### ❌ Problema: "Página no encontrada" en http://localhost:4200

**Solución:**
1. Verifica que Terminal 2 (Frontend) está corriendo
2. Busca el mensaje: `Application bundle generation complete`
3. Si no ves ese mensaje, presiona `Ctrl+C` y ejecuta nuevamente:
   ```bash
   cd ShoppingCartUI
   npm start
   ```
4. Espera 10-15 segundos después del mensaje de compilación
5. Recarga el navegador: `Ctrl+F5`

---

### ❌ Problema: "Cannot GET /swagger"

**Solución:**
1. Verifica que Terminal 1 (Backend) está corriendo
2. Busca el mensaje: `Now listening on: http://localhost:5276`
3. Si no ves ese mensaje, presiona `Ctrl+C` y ejecuta:
   ```bash
   cd ShoppingCartApi
   dotnet run
   ```
4. Espera 5-10 segundos
5. Intenta acceder nuevamente a: http://localhost:5276/swagger

---

### ❌ Problema: Error al hacer login

**Respuesta rápida:**
- **Usuario:** `testuser`
- **Contraseña:** `Password123!`

**Si sigue sin funcionar:**
1. Abre la Consola del navegador: `F12` → pestaña **Console**
2. Busca errores rojos en la consola
3. Copia el error y búscalo en este documento

---

### ❌ Problema: "npm: command not found"

**Solución:**
1. Verifica que Node.js está instalado:
   ```bash
   node --version
   npm --version
   ```
2. Si no sale versión, descarga Node.js en: https://nodejs.org/
3. Reinicia VS Code después de instalar
4. Intenta nuevamente `npm install`

---

### ❌ Problema: "dotnet: command not found"

**Solución:**
1. Verifica que .NET SDK está instalado:
   ```bash
   dotnet --version
   ```
2. Si no sale versión, descarga .NET SDK en: https://dotnet.microsoft.com/download
3. Reinicia VS Code después de instalar
4. Intenta nuevamente `dotnet run`

---

### ❌ Problema: Puerto 4200 ya está en uso

**Solución rápida:**
1. Presiona `Ctrl+C` en Terminal 2
2. Ejecuta:
   ```bash
   npm start -- --port 4300
   ```
3. Accede a: `http://localhost:4300`

---

##  Pruebas Unitarias

```bash
# Ejecutar tests
dotnet test ShoppingCartApi.Tests/ShoppingCartApi.Tests.csproj

# Con cobertura
dotnet test ShoppingCartApi.Tests/ShoppingCartApi.Tests.csproj \
  --collect "XPlat Code Coverage"

# Generar reporte
reportgenerator -reports:ShoppingCartApi.Tests/TestResults/*/coverage.cobertura.xml \
  -targetdir:coverage-report -reporttypes:Html

# Abrir reporte
coverage-report/index.html
```

## 📚 Probar la API con Swagger (Sin escribir código)

Swagger es una interfaz gráfica para probar los endpoints de la API sin escribir código.

### Paso 1: Abre Swagger

Cuando el backend está ejecutándose, abre:
```
http://localhost:5276/swagger
```

Verás todos los endpoints disponibles agrupados por tipo.

### Paso 2: Registra un usuario (Opcional)

1. Ve a **Auth** (azul)
2. Abre **POST /api/Auth/register**
3. Presiona "Try it out"
4. Reemplaza con:
   ```json
   {
     "username": "nuevouser",
     "password": "Password123!"
   }
   ```
5. Presiona "Execute"

### Paso 3: Inicia sesión (Obligatorio)

1. Ve a **Auth** (azul)
2. Abre **POST /api/Auth/login**
3. Presiona "Try it out"
4. Reemplaza con:
   ```json
   {
     "username": "testuser",
     "password": "Password123!"
   }
   ```
5. Presiona "Execute"
6. En la respuesta, copia el campo `token` completo (sin las comillas)

### Paso 4: Usa el token en otros endpoints

1. Presiona el botón **🔓 Authorize** en la parte superior
2. Pega el token anterior en el campo de texto
3. Presiona "Authorize"
4. Presiona "Close"

Ahora todos los endpoints protegidos funcionarán automáticamente.

### Paso 5: Prueba otros endpoints

Ya puedes probar:
- `GET /api/ShoppingCart/get-cart` - Ver tu carrito
- `POST /api/ShoppingCart/add-product` - Agregar producto
- `PUT /api/ShoppingCart/update-quantity` - Cambiar cantidad
- `DELETE /api/ShoppingCart/remove-product/{id}` - Eliminar producto

---

## 📚 Documentación

### 📖 Guías Completas

Encontrarás documentación detallada en los siguientes archivos:

|-----------|-------------|-----------|
| **[COMO_VERIFICAR_COMPRAS.md](./COMO_VERIFICAR_COMPRAS.md)** | 📋 Cómo ver y verificar tus órdenes compradas. Incluye ubicaciones del historial, qué información verás, y cómo confirmar que un producto fue comprado | 👤 Usuarios finales |
| **[GUIA_RAPIDA.md](./GUIA_RAPIDA.md)** | ⚡ Guía rápida de 5 minutos. Cómo usar el carrito, historial y verificación de compras | 👤 Usuarios nuevos |
| **[CAMBIOS_REALIZADOS.md](./CAMBIOS_REALIZADOS.md)** | 🔧 Documentación técnica detallada de todos los cambios implementados | 👨‍💻 Desarrolladores |
| **[RESUMEN_EJECUTIVO.md](./RESUMEN_EJECUTIVO.md)** | 📊 Resumen de características, estadísticas y mejoras del proyecto | 👨‍💼 Gerentes/Testers |
| **[INDICE_DOCUMENTACION.md](./INDICE_DOCUMENTACION.md)** | 📚 Índice completo de toda la documentación con búsqueda rápida | 🔍 Todos |

### 🎯 Acceso Rápido por Caso de Uso

**¿Cómo verifico si un producto está comprado?**  
👉 [COMO_VERIFICAR_COMPRAS.md](./COMO_VERIFICAR_COMPRAS.md)

**¿Dónde veo mi historial de órdenes?**  
👉 [GUIA_RAPIDA.md](./GUIA_RAPIDA.md#-historial-de-órdenes-url-order-history)

**¿Qué cambió en esta versión?**  
👉 [CAMBIOS_REALIZADOS.md](./CAMBIOS_REALIZADOS.md)

---

## 🎓 Próximos Pasos (Aprende más)

### Para Desarrolladores .NET

Estas carpetas contienen el código del backend:

| Carpeta | Qué contiene | Para aprender |
|---------|------------|---------------|
| **Domain/** | Entidades (Product, User) | Lógica empresarial |
| **Application/** | Casos de uso (Commands, Queries) | Patrones CQRS |
| **Infrastructure/** | Repositorios y base de datos | Acceso a datos |
| **Presentation/** | Controladores (APIs REST) | Cómo funcionan los endpoints |
| **Middleware/** | Manejo de errores | Seguridad |

Lee los archivos en estas carpetas para entender la arquitectura.

### Para Desarrolladores Angular

Estas carpetas contienen el código del frontend:

| Carpeta | Qué contiene | Para aprender |
|---------|------------|---------------|
| **src/app/domain/** | Modelos y entidades | Estructuras de datos |
| **src/app/application/** | Servicios HTTP | Cómo se conecta con la API |
| **src/app/infrastructure/** | Guards, interceptores | Seguridad y autenticación |
| **src/app/presentation/** | Componentes y páginas | UI/UX |
| **src/app/shared/** | Modelos comunes | Tipos compartidos |

Lee los archivos en estas carpetas para entender la estructura.

### Recursos Útiles

- 📖 [Documentación de ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- 📖 [Documentación de Angular](https://angular.io/docs)
- 📖 [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- 📖 [JWT (JSON Web Tokens)](https://jwt.io)

---

## 📄 Licencia

Este proyecto está bajo licencia MIT.

## 👤 Autor

Desarrollado como aplicación educativa en Clean Architecture.


1.  **Registro de Usuario:** Ejecuta la petición `### Registro de usuario` para crear un nuevo usuario.
2.  **Login de Usuario:** Ejecuta la petición `### Login de usuario`. Esto te devolverá un token JWT. Copia este token.
3.  **Configurar Token:** Pega el token copiado en la variable `{{token}}` dentro del archivo `ShoppingCartApi.http` (si tu editor lo soporta, o reemplázalo manualmente en cada petición).
4.  **Operaciones del Carrito:** Ahora puedes ejecutar las peticiones para `### Añadir producto al carrito`, `### Actualizar cantidad de producto en el carrito`, `### Eliminar producto del carrito` y `### Obtener contenido del carrito`.

    *   **Nota sobre `AddProductToCart`:** El cuerpo de la solicitud incluye `selectedAttributeGroups` para demostrar la validación de atributos. Asegúrate de que los `productId`, `groupAttributeId` y `attributeId` existan en tu base de datos o repositorio en memoria para que la validación sea exitosa.

## Validaciones de Atributos de Producto

La API incluye validaciones robustas al añadir productos al carrito, asegurando la consistencia y las cantidades de los grupos y atributos seleccionados.

## Pruebas Unitarias y Cobertura de Código

Para ejecutar las pruebas unitarias y generar un informe de cobertura de código:

1.  **Navega al directorio raíz del proyecto** (donde se encuentra el archivo `ShoppingCartApiSolution.sln`):
    ```bash
    cd ..
    ```

2.  **Añade una referencia del proyecto principal al proyecto de pruebas**:
    ```bash
    dotnet add ShoppingCartApi.Tests/ShoppingCartApi.Tests.csproj reference ShoppingCartApi/ShoppingCartApi.csproj
    ```

3.  **Instala las herramientas de cobertura y generación de informes**:
    ```bash
    dotnet add ShoppingCartApi.Tests/ShoppingCartApi.Tests.csproj package Moq
    dotnet add ShoppingCartApi.Tests/ShoppingCartApi.Tests.csproj package FluentAssertions
    dotnet tool install -g dotnet-reportgenerator-globaltool
    ```

4.  **Ejecuta las pruebas y recolecta la cobertura**:
    ```bash
    dotnet test ShoppingCartApi.Tests/ShoppingCartApi.Tests.csproj --collect "XPlat Code Coverage"
    ```

5.  **Genera el informe de cobertura HTML**:
    ```bash
    reportgenerator -reports:ShoppingCartApi.Tests/TestResults/*/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
    ```

6.  **Abre el informe de cobertura**:
    Abre el archivo `coverage-report/index.html` en tu navegador para ver el informe detallado de cobertura.

## Encabezados de Seguridad HTTP (OWASP)

La aplicación también incluye encabezados de seguridad HTTP para mitigar algunas vulnerabilidades comunes de OWASP, como `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` y `Permissions-Policy`.

## Configuración de CORS

La API está configurada con una política de CORS (`AllowSpecificOrigin`) para permitir solicitudes desde orígenes específicos (`http://localhost:3000`, `http://localhost:5000`). Esta configuración permite cualquier encabezado y método, y soporta el envío de credenciales (cookies, encabezados de autorización).

## Resumen de Seguridad (OWASP)

Se ha realizado una revisión de seguridad del proyecto `ShoppingCartApi` basándose en los estándares OWASP. A continuación, se presenta un resumen de los hallazgos y recomendaciones clave:

**1. Autenticación y Gestión de Sesiones (OWASP A07:2021 - Identification and Authentication Failures)**
*   **Fortaleza de la clave JWT:** Asegurarse de que `Jwt:Key` en `appsettings.json` sea una clave fuerte y compleja.
*   **Rotación de claves JWT:** Implementar un mecanismo para rotar periódicamente la clave JWT.
*   **Limitación de intentos de inicio de sesión:** **IMPLEMENTADO.** Se ha añadido lógica para bloquear cuentas después de un número configurable de intentos fallidos de inicio de sesión (`MaxFailedAttempts`, `LockoutEnd`).

**2. Control de Acceso (OWASP A01:2021 - Broken Access Control)**
*   **Autorización granular:** Implementar autorización basada en roles o políticas si es necesario.
*   **Validación de propiedad:** Asegurarse de que los recursos manipulados pertenezcan al usuario autenticado.

**3. Inyección (OWASP A03:2021 - Injection)**
*   **Validación de entradas:** **IMPLEMENTADO.** Se ha añadido validación exhaustiva de todas las entradas de usuario en los DTOs de comandos y modelos de solicitud (`RegisterRequest`, `LoginRequest`) utilizando `System.ComponentModel.DataAnnotations` y `ValidationException` personalizada.
*   **Sanitización de entradas:** Considerar la sanitización para cualquier entrada de texto libre que pueda ser mostrada o almacenada.

**4. Configuración de Seguridad Incorrecta (OWASP A05:2021 - Security Misconfiguration)**
*   **Clave JWT en producción:** La clave JWT no debe estar directamente en `appsettings.json` en un entorno de producción.
*   **Deshabilitar Swagger en producción:** Swagger no debería estar habilitado en entornos de producción accesibles públicamente.
*   **Políticas de CORS:** Asegurarse de que los orígenes permitidos en la política de CORS sean los correctos para el entorno de producción y no se utilicen comodines (`*`) a menos que sea estrictamente necesario.

**5. Exposición de Datos Sensibles (OWASP A04:2021 - Insecure Design / A02:2021 - Cryptographic Failures)**
*   **Protección de datos en tránsito:** **IMPLEMENTADO.** Se ha activado la redirección HTTPS (`app.UseHttpsRedirection()`) para asegurar que todas las comunicaciones con la API se realicen a través de HTTPS.
*   **Almacenamiento de datos sensibles:** Revisar si hay otros datos sensibles que se almacenen sin cifrado o con cifrado débil.
*   **Logging:** Asegurarse de que los logs no registren información sensible como contraseñas o tokens JWT completos.

**6. Fallas de Integridad de Software y Datos (OWASP A08:2021 - Software and Data Integrity Failures):**
*   **Manejo de Excepciones:** **IMPLEMENTADO.** Se ha añadido un `ExceptionHandlingMiddleware` global para capturar y manejar excepciones de forma consistente, devolviendo respuestas HTTP estandarizadas (400 para validación, 404 para no encontrado, 500 para errores internos) y evitando la exposición de detalles sensibles en producción.

**7. Fallas de Registro y Monitoreo de Seguridad (OWASP A09:2021 - Security Logging and Monitoring Failures):**
*   **Manejo de Excepciones:** El `ExceptionHandlingMiddleware` contribuye a un registro más seguro al no exponer detalles internos. Se recomienda implementar un sistema de logging y monitoreo robusto.

**8. Server-Side Request Forgery (SSRF) (OWASP A10:2021 - Server-Side Request Forgery):**
*   No aplicable directamente en la funcionalidad actual, ya que no hay llamadas a recursos externos basadas en entradas de usuario.

**9. Componentes Vulnerables y Obsoletos (OWASP A06:2021 - Vulnerable and Outdated Components):**
*   Se recomienda realizar auditorías periódicas de las dependencias del proyecto.

## Despliegue en la Nube (AWS o Azure)

Esta sección proporciona una guía general para desplegar la API en plataformas de nube como AWS o Azure. Los pasos específicos pueden variar según los servicios exactos que se utilicen (ej., AWS Elastic Beanstalk, Azure App Service, contenedores con Docker/Kubernetes).

### Despliegue en AWS (Ejemplo con Elastic Beanstalk)

1.  **Instalar AWS CLI y AWS Toolkit para .NET (opcional):**
    Asegúrate de tener las herramientas necesarias configuradas en tu entorno local.

2.  **Crear un entorno Elastic Beanstalk:**
    *   Navega a la consola de AWS Elastic Beanstalk.
    *   Crea una nueva aplicación y un nuevo entorno.
    *   Selecciona la plataforma `.NET on Linux` (o `.NET on Windows Server` si prefieres).
    *   Configura los detalles del entorno (nombre, dominio, tipo de instancia, etc.).

3.  **Preparar la aplicación para el despliegue:**
    Desde el directorio raíz del proyecto (`Backend Prueba técnica`), publica la aplicación:
    ```bash
    dotnet publish -c Release -o out
    ```
    Esto creará una carpeta `out` con los archivos listos para el despliegue.

4.  **Empaquetar la aplicación:**
    Crea un archivo `.zip` con el contenido de la carpeta `out`.
    ```bash
    cd out
    zip -r deploy.zip .
    ```

5.  **Desplegar la aplicación:**
    Sube el archivo `deploy.zip` a tu entorno de Elastic Beanstalk a través de la consola de AWS o usando AWS CLI:
    ```bash
    aws elasticbeanstalk create-application-version --application-name tu-aplicacion --version-label v1 --source-bundle S3Bucket=tu-bucket-s3,S3Key=deploy.zip
    aws elasticbeanstalk update-environment --environment-name tu-entorno --version-label v1
    ```
    (Asegúrate de reemplazar `tu-aplicacion`, `tu-entorno` y `tu-bucket-s3` con tus valores reales).

6.  **Configurar variables de entorno:**
    En la configuración de Elastic Beanstalk, añade las variables de entorno necesarias para la aplicación, como `ASPNETCORE_ENVIRONMENT` (ej., `Production`), `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, etc.

### Despliegue en Azure (Ejemplo con Azure App Service)

1.  **Instalar Azure CLI y Azure App Service Extension para VS Code (opcional):**
    Asegúrate de tener las herramientas necesarias configuradas.

2.  **Crear un Azure App Service:**
    *   Navega a la consola de Azure Portal.
    *   Crea un nuevo "App Service".
    *   Selecciona la suscripción, grupo de recursos, nombre de la aplicación, pila de tiempo de ejecución (.NET), sistema operativo (Linux o Windows) y región.

3.  **Preparar la aplicación para el despliegue:**
    Desde el directorio raíz del proyecto (`Backend Prueba técnica`), publica la aplicación:
    ```bash
    dotnet publish -c Release -o out
    ```

4.  **Desplegar la aplicación:**
    Puedes desplegar la aplicación usando Azure CLI:
    ```bash
    az webapp up --resource-group tu-grupo-recursos --name tu-app-service --runtime DOTNET|9.0 --source out
    ```
    O a través de Visual Studio Code con la extensión de Azure App Service, seleccionando la carpeta `out` para el despliegue.

5.  **Configurar variables de entorno:**
    En Azure Portal, navega a tu App Service, luego a "Configuración" -> "Configuración de la aplicación" y añade las variables de entorno (Application settings) para la aplicación, como `ASPNETCORE_ENVIRONMENT` (ej., `Production`), `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, etc.

### Consideraciones Adicionales para el Despliegue:

*   **Base de Datos:** Para un entorno de producción, reemplazar los repositorios en memoria con una base de datos real (ej., AWS RDS, Azure SQL Database) y configurar las cadenas de conexión.
*   **Secretos:** No almacenar secretos sensibles (como `Jwt:Key`) directamente en el código o en `appsettings.json` en producción. Utiliza servicios de gestión de secretos de la nube (ej., AWS Secrets Manager, Azure Key Vault).
*   **HTTPS:** Asegúrate de que HTTPS esté configurado y forzado en tu entorno de producción.
*   **Logging y Monitoreo:** Configura servicios de logging y monitoreo de la nube (ej., AWS CloudWatch, Azure Monitor) para la aplicación.
*   **Escalabilidad:** Configura el autoescalado según la demanda.
*   **Dominio Personalizado:** Configura un dominio personalizado y certificados SSL/TLS.

---

## 🎨 ShoppingCartUI - Proyecto Angular

La carpeta **ShoppingCartUI** contiene la interfaz de usuario desarrollada con Angular 20+ con arquitectura limpia, patrones de diseño avanzados y autenticación JWT.

### 📋 Requisitos Previos para Angular UI

*   Node.js 18+ 
*   npm 9+ o yarn
*   Angular CLI 20+

### ⚡ Pasos Rápidos para Ejecutar Angular UI

#### 1. Instalar Dependencias

Desde la carpeta raíz del proyecto, navega a ShoppingCartUI e instala las dependencias:

```bash
cd ShoppingCartUI
npm install
```

#### 2. Configurar URL de API

Edita el archivo `src/environments/environment.development.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // Asegúrate de que coincida con la URL de tu API .NET Core
};
```

Para producción, edita `src/environments/environment.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com/api'
};
```

#### 3. Ejecutar la Aplicación en Desarrollo

```bash
ng serve
```

La aplicación estará disponible en: **http://localhost:4200**

### 🚀 Flujo de Uso de la Aplicación

1. **Auto-redirección a Login**
   - Si no estás autenticado, serás redirigido automáticamente a `/auth/login`

2. **Login**
   - Ingresa tu email y contraseña registrados
   - Se guardará automáticamente el token JWT en localStorage
   - Serás redirigido a la página de inicio (`/`)

3. **Registro**
   - Si no tienes cuenta, puedes registrarte en `/auth/register`
   - Completa los campos: nombre, email, contraseña y confirmación

4. **Acceso Protegido**
   - Las rutas están protegidas con AuthGuard
   - El token se envía automáticamente en cada petición (mediante JwtInterceptor)
   - Si el token expira, serás redirigido a login

### 📁 Estructura del Proyecto Angular

```
ShoppingCartUI/
├── src/app/
│   ├── domain/              ← Lógica de negocio pura
│   ├── application/         ← Casos de uso
│   ├── infrastructure/      ← API, Repositorios, Guards, Interceptores
│   ├── presentation/        ← Componentes y Páginas
│   └── shared/              ← Utilidades y Constantes
├── src/environments/        ← Configuración de ambientes
└── README.md               ← Documentación
```

### 🔐 Autenticación JWT

El proyecto incluye un sistema de autenticación JWT completo:

- **JwtInterceptor**: Agrega automáticamente el token Bearer a todas las peticiones
- **AuthGuard**: Protege rutas que requieren autenticación
- **NoAuthGuard**: Previene acceso a login/registro si ya estás autenticado
- **JwtTokenService**: Manage de tokens (guardar, obtener, validar expiración)

### ️ Comandos Útiles

```bash
# Ejecutar en desarrollo
ng serve

# Build para producción
ng build --configuration production

# Ejecutar tests unitarios
ng test

# Ejecutar linter
ng lint

# Generar nuevo componente
ng generate component components/my-component
```

### ⚙️ Integración con API .NET Core

Para que el proyecto Angular funcione correctamente con la API .NET Core:

1. **Asegúrate de que la API esté ejecutándose:**
   ```bash
   cd ShoppingCartApi
   dotnet run
   ```

2. **Configuración de CORS:** La API debe permitir solicitudes desde `http://localhost:4200`

3. **Endpoints Esperados:**
   - `POST /api/auth/login` - Login
   - `POST /api/auth/register` - Registro
   - `GET /api/products` - Listar productos
   - `GET /api/shopping-carts/user/{userId}` - Obtener carrito
   - Otros endpoints según los casos de uso

### 🐛 Troubleshooting

#### Error: "Cannot find module '../../../../environments/environment'"
- Verifica que la ruta de importación sea correcta: `../../../environments/environment`

#### Error 401 - No Autorizado
- Verifica que el servidor API esté en ejecución
- Revisa que las credenciales sean válidas
- Consulta `JWT_GUIDE.md` para más detalles

#### CORS Error
- Asegúrate de que la API está configurada con CORS para `http://localhost:4200`
- En ASP.NET Core, verifica la política de CORS en `Program.cs`

### 📖 Más Información

Para información detallada sobre:
- Arquitectura limpia: Ver `ShoppingCartUI/ARCHITECTURE.md`
- Patrones de diseño: Ver `ShoppingCartUI/DESIGN_PATTERNS.md`
- Autenticación JWT: Ver `ShoppingCartUI/JWT_GUIDE.md`
- Guía de inicio: Ver `ShoppingCartUI/QUICK_START.md`

---

## 🔄 Flujo General de la Aplicación

```
┌─────────────────────────────────────────┐
│      Angular UI (http://localhost:4200) │
└─────────────────────────────────────────┘
              ↓ (Peticiones HTTP)
┌─────────────────────────────────────────┐
│   .NET Core API (http://localhost:5000) │
└─────────────────────────────────────────┘
              ↓ (Acceso a Datos)
┌─────────────────────────────────────────┐
│   Base de Datos (En Memoria o Real)     │
└─────────────────────────────────────────┘
```
