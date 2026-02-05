# 📦 Sistema de Gestión de Inventario - ASP.NET Core MVC

---

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)

---

Sistema web empresarial para el control integral de inventario de productos con gestión de movimientos (entradas/salidas), autenticación basada en roles y reportería dinámica en tiempo real. Desarrollado con ASP.NET Core MVC 8.0, Entity Framework Core y SQL Server.

## ✨ Características Principales

### 🔐 **Sistema de Autenticación y Autorización**
- Autenticación basada en cookies con control de acceso por roles (Admin/User)
- Hash seguro de contraseñas mediante SHA256
- Claims-based authorization para control granular de permisos

### 📋 **Gestión Completa de Productos**
- CRUD completo con validación de códigos únicos
- Búsqueda en tiempo real por código, nombre o categoría
- Control automático de stock con cada movimiento
- Protección de integridad referencial

### 📊 **Control Inteligente de Movimientos**
- **Flujo de trabajo guiado en 3 pasos:**
  1. 🔍 **Buscar**: Localizar producto por múltiples criterios
  2. ✅ **Seleccionar**: Elegir el producto con vista previa
  3. 📝 **Registrar**: Capturar tipo, cantidad y observaciones
- Actualización transaccional de stock con rollback automático
- Validación de stock disponible para salidas
- Historial completo con timestamps automáticos

### 📈 **Dashboard de Administración**
- Estadísticas en tiempo real del inventario
- Valor total del inventario y stock disponible
- Alertas de productos con stock bajo
- Top 10 productos más movidos
- Gráficas interactivas de movimientos mensuales
- Distribución por categorías

### 👥 **Gestión de Usuarios**
- CRUD completo de usuarios (solo Admin)
- Cambio de contraseñas y gestión de roles
- Validaciones de seguridad (no eliminar último admin)

### 📊 **Reportería Avanzada**
- Filtros dinámicos por rango de fechas y tipo de movimiento
- Cálculos automáticos: entradas, salidas y balance
- Reporte de inventario valorizado
- Productos sin movimientos recientes

---

## 🏗️ Arquitectura del Sistema
```
┌─────────────────────────────────────────────────────────────────┐
│                     CAPA DE PRESENTACIÓN                        │
│                    (Razor Views + Bootstrap)                    │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│                   CAPA DE CONTROLADORES                         │
│     ProductosController • MovimientosController                 │
│     AdminController • UsuariosController • LoginController      │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│              CAPA DE ACCESO A DATOS                             │
│              (Entity Framework Core + LINQ)                     │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│                   BASE DE DATOS                                 │
│                    (SQL Server)                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Tecnologías Utilizadas

| Categoría | Tecnología | Propósito |
|-----------|-----------|-----------|
| **Framework** | ASP.NET Core MVC 8.0 | Framework web principal |
| **Lenguaje** | C# 12 | Lenguaje de programación |
| **ORM** | Entity Framework Core 8.0 | Mapeo objeto-relacional |
| **Base de Datos** | SQL Server 2019+ | Almacenamiento de datos |
| **Frontend** | Razor Pages + Bootstrap 5.3 | Vistas y diseño responsive |
| **Autenticación** | Cookie Authentication | Gestión de sesiones |
| **Gráficas** | Chart.js | Visualización de datos |
| **Iconos** | Bootstrap Icons | Interfaz visual |

---

## 📦 Requisitos

- **.NET 8.0 SDK** o superior
- **SQL Server 2019+** o SQL Server Express
- **Visual Studio 2022** (recomendado) o VS Code

---

## 🚀 Instalación

### 1. Clonar el repositorio
```bash
git clone https://github.com/tu-usuario/sistema-inventario-mvc.git
cd sistema-inventario-mvc
```

### 2. Restaurar dependencias
```bash
dotnet restore
```

---

## ⚙️ Configuración

### 1. Configurar la cadena de conexión

Edita el archivo `appsettings.json` con tu información de SQL Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ExamenMVCDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Ejemplos de cadenas de conexión:**
```json
// SQL Server Express (más común en desarrollo)
"Server=localhost\\SQLEXPRESS;Database=ExamenMVCDB;Trusted_Connection=True;TrustServerCertificate=True;"

// SQL Server con autenticación Windows
"Server=localhost;Database=ExamenMVCDB;Trusted_Connection=True;TrustServerCertificate=True;"

// SQL Server con usuario y contraseña
"Server=localhost;Database=ExamenMVCDB;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True;"
```

---

### 2. Crear la base de datos

#### 📝 **Método 1: Usando el script SQL incluido (RECOMENDADO - Más Fácil)**

El repositorio incluye el archivo `ExamenMVC_Database_Script.sql` con toda la estructura de la base de datos.

**Pasos para ejecutar el script:**

1. **Abre SQL Server Management Studio (SSMS)** o Azure Data Studio
2. **Conéctate** a tu instancia de SQL Server
3. **Abre el archivo SQL** del repositorio:
```
   File → Open → File... → Selecciona ExamenMVC_Database_Script.sql
```
4. **Ejecuta el script:**
```
   Presiona F5 o click en "Execute"
```
5. **¡Listo!** La base de datos `ExamenMVCDB` estará completamente configurada

**Desde línea de comandos:**
```bash
# Windows con autenticación Windows
sqlcmd -S localhost\SQLEXPRESS -E -i ExamenMVC_Database_Script.sql

# Con usuario y contraseña
sqlcmd -S localhost -U sa -P tuPassword -i ExamenMVC_Database_Script.sql
```

**El script crea automáticamente:**
- ✅ Base de datos `ExamenMVCDB`
- ✅ 3 tablas: `Productos`, `Usuarios`, `Movimientos`
- ✅ 2 usuarios de prueba (`admin/admin123` y `usuario/usuario123`)
- ✅ 15 productos de ejemplo en diferentes categorías
- ✅ 15 movimientos de inventario de muestra
- ✅ 2 vistas útiles para reportes
- ✅ 2 procedimientos almacenados
- ✅ Índices optimizados para búsquedas

> 💡 **Ventaja:** Empiezas con datos de prueba listos para explorar el sistema inmediatamente.

---

#### 🔧 **Método 2: Usando Entity Framework Migrations (Alternativo)**

Si prefieres el enfoque Code-First:
```bash
# Instalar herramientas de EF (si no las tienes)
dotnet tool install --global dotnet-ef

# Crear migración inicial
dotnet ef migrations add InitialCreate

# Aplicar migración a la base de datos
dotnet ef database update
```

> ⚠️ **Nota:** Con este método necesitarás crear manualmente los usuarios y datos de prueba.

---

### 3. Verificar la instalación

Ejecuta estas consultas en SSMS para confirmar:
```sql
USE ExamenMVCDB;

-- Ver tablas creadas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Verificar datos insertados
SELECT 'Usuarios' AS Tabla, COUNT(*) AS Registros FROM Usuarios
UNION ALL
SELECT 'Productos', COUNT(*) FROM Productos
UNION ALL
SELECT 'Movimientos', COUNT(*) FROM Movimientos;
```

**Resultado esperado:**
```
Tabla         Registros
─────────────────────────
Usuarios            2
Productos          15
Movimientos        15
```

**Credenciales de acceso:**
- 👤 **Admin:** `admin` / `admin123`
- 👤 **Usuario:** `usuario` / `usuario123`

---

### 4. Ejecutar la aplicación

Presiona el botón de **ejecutar** en Visual Studio.

O también puedes ejecutar:
```bash
dotnet run
```

O presiona **F5** en Visual Studio.

Accede a: `https://localhost:5001`

---

## 💻 Funcionalidades Principales

- 📦 **Gestión de Productos:** CRUD completo con búsqueda en tiempo real
- 📊 **Registro de Movimientos:** Flujo guiado de 3 pasos
- 📈 **Dashboard Admin:** Estadísticas, gráficas y alertas
- 👥 **Gestión de Usuarios:** Control de acceso por roles
- 📑 **Reportes Dinámicos:** Filtros por fecha, tipo y totalizadores automáticos

---

## 📂 Estructura del Proyecto
```
ExamenMVC/
├── Controllers/               # Lógica de negocio MVC
│   ├── AdminController.cs    # Dashboard y reportes
│   ├── ProductosController.cs
│   ├── MovimientosController.cs
│   └── UsuariosController.cs
├── Models/                   # Entidades y ViewModels
├── Views/                    # Vistas Razor
│   ├── Admin/
│   │   └── Dashboard.cshtml
│   ├── Productos/
│   ├── Movimientos/
│   └── Usuarios/
├── Data/                     # EF Core DbContext
├── Services/                 # Servicios de aplicación
└── ExamenMVC_Database_Script.sql  # Script de base de datos
```

---

## 🔒 Seguridad

### Implementado
✅ Autenticación basada en cookies  
✅ Autorización por roles (Admin/User)  
✅ Protección CSRF con tokens anti-falsificación  
✅ Contraseñas hasheadas con SHA256  
✅ Validación dual (cliente + servidor)  
✅ Consultas parametrizadas (EF Core)  
✅ Validaciones de integridad referencial  

---

## 📏 Reglas de Negocio

### Productos
- Códigos únicos obligatorios
- Si hay stock, el precio debe ser mayor que 0
- No se pueden eliminar productos con movimientos

### Movimientos
- Solo tipos "Entrada" o "Salida"
- Las salidas no pueden exceder el stock disponible
- Registro inmutable (no se pueden editar ni eliminar)
- Fecha automática en UTC

### Usuarios
- Nombres de usuario únicos
- Contraseñas mínimo 6 caracteres
- No se puede eliminar el último administrador

---

## 🎯 Demostración de Capacidades

Este proyecto demuestra:
- ✅ Arquitectura MVC con separación de responsabilidades
- ✅ Manejo de transacciones y consistencia de datos
- ✅ Implementación de patrones de diseño (Repository, ViewModel)
- ✅ Autenticación y autorización robusta
- ✅ Uso avanzado de Entity Framework Core
- ✅ LINQ para consultas complejas
- ✅ Validación de datos en múltiples capas
- ✅ UI/UX moderna y responsive
- ✅ Reportería dinámica con visualización de datos

---

## 📄 Licencia

MIT License - Ver [LICENSE](LICENSE) para más detalles.

---

**Desarrollado por Michael Barillas** | © 2025