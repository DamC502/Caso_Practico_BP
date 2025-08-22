use db_planilla
go
--- Procedimiento almacenado para guardar colaboradores
CREATE   PROCEDURE CreateColaborador @CodColaborador BIGINT,
                                            @Nombre varchar(250),
                                            @CodJefe bigint,
                                            @IdPuesto bigint
AS
BEGIN
    SET NOCOUNT OFF;


    IF EXISTS (SELECT 1 FROM Colaborador WHERE Codigo_colaborador = @CodColaborador)
        BEGIN
            ;THROW 90021, 'El código del colaborador ya esta siendo utilizado.', 1;
        END

    IF NOT EXISTS(SELECT 1 FROM Puesto WHERE PK_Puesto = @IdPuesto)
        BEGIN
            ;THROW 90022, 'El puesto no existe.', 1;
        end


    if @CodJefe is not null
        begin
            IF NOT EXISTS(SELECT 1 FROM Colaborador WHERE Codigo_colaborador = @CodJefe)
                BEGIN
                    ;
                    THROW 90023, 'El Jefe no existe.', 1;
                end
        end

    declare @FKJefe bigint;

    if @CodJefe is not null
        begin
            Select @FKJefe = PK_Colaborador
            From Colaborador
            WHERE Codigo_colaborador = @CodJefe
        end
    else
        begin
            select @FKJefe = NULL
        end

    begin transaction insertNewColaborador
        begin try
            insert into Colaborador(FK_Puesto, FK_Jefe, Nombre, Codigo_colaborador, fecha_creacion)
            values (@IdPuesto, @CodJefe, @Nombre, @CodColaborador, GETDATE())
            commit transaction
        end try
        begin catch
            rollback transaction insertNewColaborador;
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();

            DECLARE @ErrorState INT = ERROR_STATE();
            THROW 900024, @ErrorMessage, @ErrorState
        end catch
end
go
--- Procedimiento almacenado para eliminar colaboradores

CREATE   PROCEDURE DeleteColaborador @IdColaborador BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Colaborador WHERE Codigo_colaborador = @IdColaborador)
        BEGIN
            ;THROW 90001, 'El colaborador no existe.', 1;
        END

    DECLARE @Jefe BIGINT;
    DECLARE @PK_Colaborador BIGINT;


    SELECT @Jefe = FK_Jefe, @PK_Colaborador = PK_Colaborador
    FROM Colaborador
    WHERE Codigo_colaborador = @IdColaborador;

    begin transaction deleteColaborador
        begin try
            UPDATE Colaborador
            SET FK_Jefe      = @Jefe,
                fecha_update = GETDATE()
            WHERE FK_Jefe = @PK_Colaborador;

            UPDATE Colaborador
            SET cod_estado   = 0,
                fecha_update = GETDATE()
            WHERE Codigo_colaborador = @IdColaborador;

            commit;
        end try
        begin catch
            rollback transaction deleteColaborador;
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            DECLARE @ErrorState INT = ERROR_STATE();

            THROW 90001, @ErrorMessage, @ErrorState
        end catch
end
go
--- Procedimiento almacenado para visualizar colaboradores de forma jerárquica 
CREATE PROCEDURE GetAllColaborador
AS
BEGIN
    SET NOCOUNT ON;


    WITH planilla (PK_colaborador, codigo_colaborador, colaborador,
                   nivel, FK_jefe, codigo_puesto, ruta_orden, codigo_jefe) AS (SELECT C.PK_Colaborador,
                                                                                      C.Codigo_colaborador,
                                                                                      C.Nombre                                                    AS colaborador,
                                                                                      -- CAST(C.Nombre AS nvarchar(max)) AS jerarquia,
                                                                                      0                                                           AS nivel,
                                                                                      C.FK_Jefe                                                   AS FK_jefe,
                                                                                      C.FK_Puesto                                                 as codigo_puesto,
                                                                                      CAST(CAST(C.PK_Colaborador AS BINARY(8)) AS VARBINARY(MAX)) AS ruta_orden,
                                                                                      cast(-1 as bigint)                                          as codigo_jefe
                                                                               FROM Colaborador C
                                                                               WHERE C.FK_Jefe IS NULL
                                                                                 and cod_estado = 1

                                                                               UNION ALL

                                                                               -- Miembro recursivo
                                                                               SELECT C.PK_Colaborador,
                                                                                      C.Codigo_colaborador,
                                                                                      C.Nombre,
                                                                                      -- CAST(p.jerarquia + ' > ' + C.Nombre AS nvarchar(max)) AS jerarquia,
                                                                                      p.nivel + 1,
                                                                                      C.FK_Jefe,
                                                                                      -- Concatenamos el PK actual al path binario del jefe,
                                                                                      C.FK_Puesto          as codigo_puesto,
                                                                                      p.ruta_orden +
                                                                                      CAST(CAST(C.PK_Colaborador AS BINARY(8)) AS VARBINARY(8)),
                                                                                      p.Codigo_colaborador as codigo_jefe
                                                                               FROM Colaborador C
                                                                                        JOIN planilla p ON C.FK_Jefe = p.PK_colaborador and C.cod_estado = 1)
    SELECT
--     PK_colaborador,
codigo_colaborador,
colaborador as colaborador_nombre,
nivel,
codigo_jefe,
Puesto, p.PK_Puesto as Id_Puesto
    --     colaborador,
--     nivel,
--     codigo_jefe
    -- CONVERT(VARCHAR(MAX), ruta_orden, 1) AS HexRepresentation
    FROM planilla
             left join puesto p on p.PK_Puesto = planilla.codigo_puesto
    ORDER BY ruta_orden;
end;
go

--- Procedimiento almacenado para visualizar todos los puestos diponibles

CREATE PROCEDURE GetAllPuesto
AS
BEGIN
    SET NOCOUNT ON;
    select PK_Puesto as IdPuesto, Puesto
    from Puesto;

    end
go

CREATE   PROCEDURE UpdateColaborador @OldCodColaborador BIGINT,
                                            @CodColaborador BIGINT,
                                            @Nombre varchar(250),
                                            @CodJefe bigint,
                                            @IdPuesto bigint
AS
BEGIN
    SET NOCOUNT OFF;


    IF NOT EXISTS (SELECT 1 FROM Colaborador WHERE Codigo_colaborador = @OldCodColaborador)
        BEGIN
            ;THROW 90011, 'El colaborador no existe.', 1;
        END

    IF NOT EXISTS(SELECT 1 FROM Puesto WHERE PK_Puesto = @IdPuesto)
        BEGIN
            ;THROW 90012, 'El puesto no existe.', 1;
        end


    if @CodJefe is not null
        begin
            IF NOT EXISTS(SELECT 1 FROM Colaborador WHERE Codigo_colaborador = @CodJefe)
                BEGIN
                    ;
                    THROW 90013, 'El Jefe no existe.', 1;
                end
        end

    IF @OldCodColaborador <> @CodColaborador
        begin
            IF EXISTS(select 1 from Colaborador where Codigo_colaborador = @CodColaborador)
                Begin
                    THROW 90014, 'Este código ya pertenece a otro colaborador, no es posible actualizar ', 1;
                end
        end

    declare @PKColaborador bigint;
    declare @FKJefe bigint;

    SELECT @PKColaborador = PK_Colaborador
    FROM Colaborador
    WHERE Codigo_colaborador = @OldCodColaborador;

    if @CodJefe is not null
        begin
            Select @FKJefe = PK_Colaborador
            From Colaborador
            WHERE Codigo_colaborador = @CodJefe
        end
    else
        begin
            select @FKJefe = NULL
        end


    begin transaction updateColaborador
        begin try
            update Colaborador
            set Codigo_colaborador = @CodColaborador,
                FK_Jefe            = @FKJefe,
                Nombre             = @Nombre,
                FK_Puesto          = @IdPuesto,
                fecha_update       = GETDATE()
            where PK_Colaborador = @PKColaborador
            commit
        end try
        begin catch
            rollback transaction updateColaborador;
            DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
            DECLARE @ErrorState INT = ERROR_STATE();

            THROW 900015, @ErrorMessage, @ErrorState
        end catch
end
go;

