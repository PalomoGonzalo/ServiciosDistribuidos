CREATE TABLE Productos (
   Id int PRIMARY KEY,
   Id_Producto int,
   Nombre varchar(50) NOT NULL,
   Descripcion varchar(200),
   Precio decimal(10, 2) NOT NULL,
   Stock int NOT NULL,
   CategoriaId int NOT NULL,
   Fecha_Creacion date not null,
   Fecha_Modificacion date
   CONSTRAINT FK_Productos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);


insert into Categorias (Id,Nombre,Descripcion) values (1,'Electrodomesticos', 'Producto que requiere energia'); 