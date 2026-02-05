-- =============================================
-- Script de Base de Datos para ExamenMVC
-- Sistema de Inventario con Movimientos
-- SQL Server 2019+
-- =============================================

-- =============================================
-- 1. CREAR BASE DE DATOS
-- =============================================
USE master;
GO

-- Eliminar base de datos si existe (¡CUIDADO EN PRODUCCIÓN!)
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ExamenMVCDB')
BEGIN
    ALTER DATABASE ExamenMVCDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ExamenMVCDB;
END
GO

-- Crear la base de datos
CREATE DATABASE ExamenMVCDB;
GO

-- Usar la base de datos
USE ExamenMVCDB;
GO

PRINT 'Base de datos ExamenMVCDB creada exitosamente.';
GO

-- =============================================
-- 2. CREAR TABLAS
-- =============================================

-- ---------------------------------------------
-- Tabla: Productos
-- ---------------------------------------------
CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(30) NOT NULL UNIQUE,
    Nombre NVARCHAR(120) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (Precio >= 0 AND Precio <= 9999999),
    Categoria NVARCHAR(60) NOT NULL,
    Stock INT NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    Activo BIT NOT NULL DEFAULT 1
);
GO

PRINT 'Tabla Productos creada.';
GO

-- Índices para optimizar búsquedas
CREATE INDEX IX_Productos_Codigo ON Productos(Codigo);
CREATE INDEX IX_Productos_Nombre ON Productos(Nombre);
CREATE INDEX IX_Productos_Categoria ON Productos(Categoria);
CREATE INDEX IX_Productos_Activo ON Productos(Activo);
GO

-- ---------------------------------------------
-- Tabla: Usuarios
-- ---------------------------------------------
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    Role NVARCHAR(20) NULL DEFAULT 'User'
);
GO

PRINT 'Tabla Usuarios creada.';
GO

-- Índice para búsqueda rápida de usuario
CREATE INDEX IX_Usuarios_UserName ON Usuarios(UserName);
GO

-- ---------------------------------------------
-- Tabla: Movimientos
-- ---------------------------------------------
CREATE TABLE Movimientos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Tipo NVARCHAR(10) NOT NULL CHECK (Tipo IN ('Entrada', 'Salida')),
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    Observacion NVARCHAR(200) NULL,
    ProductoId INT NOT NULL,
    CONSTRAINT FK_Movimientos_Productos FOREIGN KEY (ProductoId) 
        REFERENCES Productos(Id) ON DELETE NO ACTION
);
GO

PRINT 'Tabla Movimientos creada.';
GO

-- Índices para optimizar consultas
CREATE INDEX IX_Movimientos_Fecha ON Movimientos(Fecha DESC);
CREATE INDEX IX_Movimientos_Tipo ON Movimientos(Tipo);
CREATE INDEX IX_Movimientos_ProductoId ON Movimientos(ProductoId);
GO

-- =============================================
-- 3. DATOS INICIALES
-- =============================================

-- ---------------------------------------------
-- Insertar Usuario Administrador
-- Password: "admin123" (SHA256 hash en minúsculas)
-- ---------------------------------------------
INSERT INTO Usuarios (UserName, PasswordHash, Role) 
VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Admin');
GO

-- Usuario de prueba: "usuario" / "usuario123"
INSERT INTO Usuarios (UserName, PasswordHash, Role) 
VALUES ('usuario', 'dfa7a2273567dcd1efffb9a46308e91c20fa13c44c3441bc69cd6a7869b3f7fd', 'User');
GO

PRINT 'Usuarios insertados:';
PRINT '  - admin / admin123 (Role: Admin)';
PRINT '  - usuario / usuario123 (Role: User)';
GO

-- ---------------------------------------------
-- Insertar Productos de Ejemplo
-- ---------------------------------------------
INSERT INTO Productos (Codigo, Nombre, Precio, Categoria, Stock, Activo) VALUES
('LAPTOP001', 'Laptop Dell Inspiron 15', 15999.99, 'Computadoras', 10, 1),
('MOUSE001', 'Mouse Logitech MX Master 3', 1899.50, 'Accesorios', 25, 1),
('TECLADO001', 'Teclado Mecánico Corsair K70', 2499.00, 'Accesorios', 15, 1),
('MONITOR001', 'Monitor Samsung 27" 4K', 8999.99, 'Monitores', 8, 1),
('IMPRESORA001', 'Impresora HP LaserJet Pro', 4500.00, 'Impresoras', 5, 1),
('CABLE001', 'Cable HDMI 2m', 199.99, 'Cables', 50, 1),
('WEBCAM001', 'Webcam Logitech C920', 1599.00, 'Accesorios', 12, 1),
('AUDIFONOS001', 'Audífonos Sony WH-1000XM4', 6499.00, 'Audio', 20, 1),
('SSD001', 'SSD Samsung 1TB NVMe', 2199.00, 'Almacenamiento', 30, 1),
('RAM001', 'Memoria RAM DDR4 16GB', 1299.50, 'Componentes', 40, 1),
('GPU001', 'Tarjeta Gráfica NVIDIA RTX 4060', 12999.00, 'Componentes', 3, 1),
('ROUTER001', 'Router TP-Link AX3000', 2899.00, 'Redes', 18, 1),
('UPS001', 'UPS APC 1000VA', 3499.00, 'Energía', 7, 1),
('DOCK001', 'Docking Station USB-C', 1799.00, 'Accesorios', 10, 1),
('MICRO001', 'Micrófono Blue Yeti', 2999.00, 'Audio', 8, 1);
GO

PRINT '15 productos de ejemplo insertados.';
GO

-- ---------------------------------------------
-- Insertar Movimientos de Ejemplo
-- ---------------------------------------------
-- Entradas iniciales (suministro de stock)
INSERT INTO Movimientos (Fecha, Tipo, Cantidad, Observacion, ProductoId) VALUES
(DATEADD(DAY, -30, GETUTCDATE()), 'Entrada', 10, 'Compra inicial de laptops', 1),
(DATEADD(DAY, -30, GETUTCDATE()), 'Entrada', 25, 'Pedido de mouses', 2),
(DATEADD(DAY, -28, GETUTCDATE()), 'Entrada', 15, 'Stock de teclados mecánicos', 3),
(DATEADD(DAY, -25, GETUTCDATE()), 'Entrada', 8, 'Monitores Samsung recibidos', 4),
(DATEADD(DAY, -25, GETUTCDATE()), 'Entrada', 5, 'Impresoras láser', 5);

-- Salidas (ventas)
INSERT INTO Movimientos (Fecha, Tipo, Cantidad, Observacion, ProductoId) VALUES
(DATEADD(DAY, -20, GETUTCDATE()), 'Salida', 2, 'Venta a cliente corporativo', 1),
(DATEADD(DAY, -18, GETUTCDATE()), 'Salida', 5, 'Pedido online', 2),
(DATEADD(DAY, -15, GETUTCDATE()), 'Salida', 3, 'Venta en tienda', 3),
(DATEADD(DAY, -12, GETUTCDATE()), 'Salida', 1, 'Proyecto empresa X', 4),
(DATEADD(DAY, -10, GETUTCDATE()), 'Entrada', 10, 'Reposición de stock', 2);

-- Movimientos recientes
INSERT INTO Movimientos (Fecha, Tipo, Cantidad, Observacion, ProductoId) VALUES
(DATEADD(DAY, -5, GETUTCDATE()), 'Salida', 2, 'Venta urgente', 1),
(DATEADD(DAY, -3, GETUTCDATE()), 'Entrada', 20, 'Nueva entrega de audífonos', 8),
(DATEADD(DAY, -2, GETUTCDATE()), 'Salida', 5, 'Pedido al por mayor', 10),
(DATEADD(DAY, -1, GETUTCDATE()), 'Entrada', 50, 'Cables HDMI recibidos', 6),
(GETUTCDATE(), 'Salida', 1, 'Venta individual', 11);
GO

PRINT '15 movimientos de ejemplo insertados.';
GO

-- =============================================
-- 4. VISTAS ÚTILES
-- =============================================

-- Vista para reporte de inventario actual
CREATE VIEW vw_InventarioActual AS
SELECT 
    p.Id,
    p.Codigo,
    p.Nombre,
    p.Categoria,
    p.Precio,
    p.Stock,
    p.Stock * p.Precio AS ValorTotal,
    p.Activo,
    COUNT(m.Id) AS TotalMovimientos,
    SUM(CASE WHEN m.Tipo = 'Entrada' THEN m.Cantidad ELSE 0 END) AS TotalEntradas,
    SUM(CASE WHEN m.Tipo = 'Salida' THEN m.Cantidad ELSE 0 END) AS TotalSalidas
FROM Productos p
LEFT JOIN Movimientos m ON p.Id = m.ProductoId
GROUP BY p.Id, p.Codigo, p.Nombre, p.Categoria, p.Precio, p.Stock, p.Activo;
GO

PRINT 'Vista vw_InventarioActual creada.';
GO

-- Vista para movimientos con detalles de producto
CREATE VIEW vw_MovimientosDetallados AS
SELECT 
    m.Id,
    m.Fecha,
    m.Tipo,
    m.Cantidad,
    m.Observacion,
    p.Codigo AS ProductoCodigo,
    p.Nombre AS ProductoNombre,
    p.Categoria AS ProductoCategoria,
    p.Precio AS ProductoPrecio,
    m.Cantidad * p.Precio AS ValorMovimiento
FROM Movimientos m
INNER JOIN Productos p ON m.ProductoId = p.Id;
GO

PRINT 'Vista vw_MovimientosDetallados creada.';
GO

-- =============================================
-- 5. PROCEDIMIENTOS ALMACENADOS ÚTILES
-- =============================================

-- SP para obtener productos con bajo stock
CREATE PROCEDURE sp_ProductosBajoStock
    @StockMinimo INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Codigo,
        Nombre,
        Categoria,
        Stock,
        Precio
    FROM Productos
    WHERE Stock < @StockMinimo AND Activo = 1
    ORDER BY Stock ASC, Nombre;
END;
GO

PRINT 'Procedimiento sp_ProductosBajoStock creado.';
GO

-- SP para reporte de movimientos por período
CREATE PROCEDURE sp_ReporteMovimientos
    @FechaDesde DATETIME2 = NULL,
    @FechaHasta DATETIME2 = NULL,
    @Tipo NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.Id,
        m.Fecha,
        m.Tipo,
        m.Cantidad,
        m.Observacion,
        p.Codigo,
        p.Nombre AS ProductoNombre,
        p.Categoria
    FROM Movimientos m
    INNER JOIN Productos p ON m.ProductoId = p.Id
    WHERE 
        (@FechaDesde IS NULL OR m.Fecha >= @FechaDesde)
        AND (@FechaHasta IS NULL OR m.Fecha <= @FechaHasta)
        AND (@Tipo IS NULL OR m.Tipo = @Tipo)
    ORDER BY m.Fecha DESC;
END;
GO

PRINT 'Procedimiento sp_ReporteMovimientos creado.';
GO

-- =============================================
-- 6. CONSULTAS DE VERIFICACIÓN
-- =============================================

PRINT '======================================';
PRINT 'VERIFICACIÓN DE DATOS';
PRINT '======================================';

-- Contar registros en cada tabla
SELECT 'Usuarios' AS Tabla, COUNT(*) AS TotalRegistros FROM Usuarios
UNION ALL
SELECT 'Productos', COUNT(*) FROM Productos
UNION ALL
SELECT 'Movimientos', COUNT(*) FROM Movimientos;
GO

-- Mostrar inventario resumido
PRINT '';
PRINT 'Inventario Resumido por Categoría:';
SELECT 
    Categoria,
    COUNT(*) AS TotalProductos,
    SUM(Stock) AS TotalStock,
    SUM(Stock * Precio) AS ValorTotal
FROM Productos
GROUP BY Categoria
ORDER BY Categoria;
GO

-- Mostrar productos con bajo stock (menos de 10)
PRINT '';
PRINT 'Productos con Stock Bajo (< 10):';
EXEC sp_ProductosBajoStock @StockMinimo = 10;
GO

PRINT '';
PRINT '======================================';
PRINT 'BASE DE DATOS CONFIGURADA EXITOSAMENTE';
PRINT '======================================';
PRINT '';
PRINT 'Credenciales de acceso:';
PRINT '  Usuario Admin: admin / admin123';
PRINT '  Usuario Normal: usuario / usuario123';
PRINT '';
PRINT 'Tablas creadas: Productos, Usuarios, Movimientos';
PRINT 'Vistas creadas: vw_InventarioActual, vw_MovimientosDetallados';
PRINT 'Procedimientos: sp_ProductosBajoStock, sp_ReporteMovimientos';
PRINT '';
GO
