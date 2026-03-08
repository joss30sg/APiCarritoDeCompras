# 👨‍💼 Portafolio de Proyectos

Bienvenido a mi portafolio profesional. Aquí encontrarás mis proyectos más relevantes desarrollados con tecnologías modernas.

---

## 🛒 1. Carrito de Compras - Full Stack

**Descripción:** Aplicación completa de e-commerce con autenticación JWT, gestión de carrito y historial de órdenes.

**Tecnologías:**
- Backend: ASP.NET Core 9, C#
- Frontend: Angular 20, TypeScript, RxJS
- Base de datos: In-Memory (desarrollo)
- Testing: xUnit, Moq, FluentAssertions

**Características:**
✅ Autenticación JWT con Login/Registro  
✅ CRUD de carrito de compras  
✅ Historial de órdenes  
✅ Validación de compras duplicadas  
✅ API REST con Swagger  
✅ Diseño responsive  
✅ Pruebas unitarias con cobertura  

**Link del Repositorio:**  
🔗 https://github.com/joss30sg/APiCarritoDeCompras

---

### 📹 DEMO - Instalar y Ejecutar Localmente

#### Requisitos Previos
```bash
# Verificar versiones instaladas
dotnet --version      # Debe ser 9.0 o superior
node --version        # Debe ser 20+ 
npm --version         # Debe ser 9+
```

#### Paso 1: Clonar el Repositorio
```bash
git clone https://github.com/joss30sg/APiCarritoDeCompras.git
cd APiCarritoDeCompras
```

#### Paso 2: Abrir 2 Terminales en VS Code
Presiona `Ctrl + Backtick (`)` dos veces

#### Terminal 1: Ejecutar Backend
```bash
cd ShoppingCartApi
dotnet run
```

**Esperar a ver:**
```
Now listening on: http://localhost:5276
```

#### Terminal 2: Ejecutar Frontend
```bash
cd Frontend/ShoppingCartUI
npm start
```

**Esperar a ver:**
```
Application bundle generation complete
```

#### Paso 3: Acceder a la Aplicación
- **Interfaz:** http://localhost:4200
- **API Swagger:** http://localhost:5276/swagger

#### Paso 4: Probar con Credenciales de Demo
```
Usuario:     testuser
Contraseña:  Password123!
```

#### Funcionalidades a Probar
1. ✅ **Login:** Inicia sesión con las credenciales arriba
2. ✅ **Ver Productos:** Verás 6 productos con imágenes reales
3. ✅ **Agregar Carrito:** Haz clic en "Agregar al Carrito"
4. ✅ **Gestionar Carrito:** Ve al icono 🛒 para ver/editar
5. ✅ **Historial:** Haz clic en "Mis Órdenes"
6. ✅ **API Swagger:** Prueba endpoints directamente en /swagger

---

### 📊 Estadísticas del Proyecto

| Métrica | Valor |
|---------|-------|
| **Líneas de código** | 5000+ |
| **Tests unitarios** | 20+ |
| **Cobertura** | 75%+ |
| **Componentes Angular** | 10+ |
| **Endpoints API** | 15+ |
| **Métodos CRUD** | Completos |

---

### 📁 Estructura del Proyecto

```
APiCarritoDeCompras/
├── ShoppingCartApi/
│   ├── Domain/           # Entidades de negocio
│   ├── Application/      # Casos de uso y lógica
│   ├── Infrastructure/   # Repositorios y persistencia
│   ├── Presentation/     # Controladores REST
│   └── Middleware/       # Manejo de errores
│
├── Frontend/ShoppingCartUI/
│   ├── src/app/
│   │   ├── domain/       # Modelos y interfaces
│   │   ├── application/  # Servicios HTTP
│   │   ├── infrastructure/ # Guards e interceptores
│   │   ├── presentation/ # Componentes
│   │   └── shared/       # Utilidades comunes
│   └── package.json
│
└── Test/
    └── ShoppingCartApi.Tests/
```

---

### 🔍 Explorar el Código

**Endpoints Principales:**

**Autenticación**
```http
POST /api/Auth/register
POST /api/Auth/login
```

**Carrito de Compras**
```http
GET    /api/ShoppingCart/get-cart
POST   /api/ShoppingCart/add-product
PUT    /api/ShoppingCart/update-quantity
DELETE /api/ShoppingCart/remove-product/{id}
```

**Órdenes**
```http
GET /api/Orders/user-orders
```

---

### 🧪 Teste el Código

```bash
# Ejecutar todas las pruebas
dotnet test Test/ShoppingCartApi.Tests.csproj

# Con cobertura de código
dotnet test Test/ShoppingCartApi.Tests.csproj \
  --collect "XPlat Code Coverage"

# Ver reporte HTML
reportgenerator -reports:Test/TestResults/*/coverage.cobertura.xml \
  -targetdir:coverage-report -reporttypes:Html
```

---

### 🎯 Aprendizajes Clave

Este proyecto demuestra:
- ✅ Arquitectura limpia (Clean Architecture)
- ✅ Patrones de diseño (Repository, Factory, etc.)
- ✅ JWT Autenticación y Autorización
- ✅ CORS y Seguridad HTTP
- ✅ Validación exhaustiva de datos
- ✅ Manejo global de excepciones
- ✅ Testing unitario y cobertura
- ✅ API REST RESTful
- ✅ Componentes reutilizables en Angular
- ✅ Interceptores HTTP para tokens

---

## 📚 Otros Proyectos

### (Próximamente)
Estoy trabajando en documentar más proyectos. Esta sección se actualizará con:
- [ ] Sistemas de autenticación avanzados
- [ ] APIs escalables con microservicios
- [ ] Aplicaciones móviles con React Native
- [ ] Data science y machine learning
- [ ] Integraciones con APIs externas

---

## 🎓 Competencias Técnicas

### Backend
- ✅ C# y .NET Core 9
- ✅ Architecture limpia y DDD
- ✅ Entity Framework Core
- ✅ JWT Autenticación
- ✅ Unit Testing (xUnit, Moq)
- ✅ SQL Server / PostgreSQL

### Frontend
- ✅ Angular 20+
- ✅ TypeScript
- ✅ RxJS y Observables
- ✅ Componentes reutilizables
- ✅ Guards y Interceptores
- ✅ Diseño responsive (Bootstrap, CSS)

### DevOps & Tools
- ✅ Git y GitHub
- ✅ Docker (básico)
- ✅ Visual Studio Code
- ✅ Postman / REST Client
- ✅ Azure / AWS (básico)

---

## 📞 Contacto y Enlaces

- **GitHub:** https://github.com/joss30sg
- **Email:** joss30sg@github.com
- **LinkedIn:** (Agregar URL)

---

## 📄 Licencia

Todos mis proyectos están bajo licencia **MIT**. Siéntete libre de usar, modificar y distribuir el código.

---

**Última actualización:** Marzo de 2026
