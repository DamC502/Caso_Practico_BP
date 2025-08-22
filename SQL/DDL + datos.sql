CREATE DATABASE db_planilla;
GO;
USE db_planilla
GO

create table dbo.Puesto
(
    PK_Puesto bigint identity
        constraint PK_Puesto
            primary key,
    Puesto    nvarchar(250) not null
)
go



create table dbo.Colaborador
(
    PK_Colaborador     bigint identity
        constraint PK_Colaborador
            primary key,
    FK_Puesto          bigint                     not null
        constraint FK_Colaborador_Puesto
            references dbo.Puesto,
    FK_Jefe            bigint
        constraint FK_Colaborador_Jefe
            references dbo.Colaborador,
    Nombre             varchar(250)               not null,
    Codigo_colaborador bigint                     not null
        constraint codigo_unico
            unique,
    cod_estado         smallint default 1         not null,
    fecha_creacion     datetime default getdate() not null,
    fecha_update       datetime
)
go

exec sp_addextendedproperty 'MS_Description', 'Relacion recursiva para el codigo jefe', 'SCHEMA', 'dbo', 'TABLE',
     'Colaborador', 'CONSTRAINT', 'FK_Colaborador_Jefe'
go





INSERT INTO dbo.Puesto ( Puesto) VALUES ( N'Gerente');
INSERT INTO dbo.Puesto ( Puesto) VALUES ( N'Sub Gerente');
INSERT INTO dbo.Puesto ( Puesto) VALUES ( N'Supervisor');


INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES (1, null, N'Pedro', 1, 1, getdate(),null);
INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES (2, 1, N'Pablo', 2, 1, getdate(), null);
INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES (3, 2, N'Juan', 3, 1, getdate(), null);
INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES (2, 1, N'José', 4, 1, getdate(), null);
INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES ( 3, 5, N'Carlos', 5, 1, getdate(), null);
INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES ( 3, 5, N'Diego', 6, 1, getdate(), null);
INSERT INTO dbo.Colaborador (FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, cod_estado, fecha_creacion, fecha_update) VALUES ( 3, 2, N'Antonio', 7, 1, getdate(), null);



