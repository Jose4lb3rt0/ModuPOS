CREATE DATABASE ModuPOS;

CREATE TABLE Productos (
	Id INT PRIMARY KEY IDENTITY(1,1),
	SKU NVARCHAR(50) UNIQUE NOT NULL,
	Nombre NVARCHAR(100) NOT NULL,
	PrecioActual DECIMAL(18, 4) NOT NULL,
	Stock INT NOT NULL DEFAULT 0,
	CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE MetodosPago (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Nombre NVARCHAR(40) NOT NULL,
);

CREATE TABLE Ventas (
	Id INT PRIMARY KEY IDENTITY(1,1),
	Folio NVARCHAR(20) UNIQUE NOT NULL,
	Fecha DATETIME DEFAULT GETDATE(),
	Subtotal DECIMAL(18, 4) NOT NULL,
	Impuestos DECIMAL(18, 4) NOT NULL,
	DescuentoTotal DECIMAL(18, 4) NOT NULL,
	Total AS (Subtotal + Impuestos - DescuentoTotal),
	Estado NVARCHAR(20) NOT NULL DEFAULT 'Completada', -- Completada, Pendiente, Cancelada, Totalmente Reembolsada, Parcialmente Reembolsada, crearé un enum en el código.
	MetodoPagoId INT NOT NULL,

	CONSTRAINT FK_MetodoPago_Ventas FOREIGN KEY (MetodoPagoId) REFERENCES MetodosPago(id)
); 

CREATE TABLE VentaDetalles (
	Id INT PRIMARY KEY IDENTITY(1,1),
	VentaId INT NOT NULL,
	ProductoId INT NOT NULL,
	Cantidad INT NOT NULL,
	PrecioUnitarioHistorico DECIMAL(18, 4) NOT NULL, -- se guarda el precio del momento
	SubtotalLinea AS (Cantidad * PrecioUnitarioHistorico),

	CONSTRAINT FK_Ventas_Detalles FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
	CONSTRAINT FK_Productos_Detalles FOREIGN KEY (ProductoId) REFERENCES Productos(Id),
);
