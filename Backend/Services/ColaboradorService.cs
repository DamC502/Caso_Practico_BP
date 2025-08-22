using Backend.Data;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Backend.Services
{
    public class ColaboradorService : IColaboradorService
    {

        private readonly ApiDbContext _context;

        public ColaboradorService( ApiDbContext context)
        {
            _context = context;
        }
        
        async Task<int> IColaboradorService.DeleteColaboradorAsync(long colaboradorId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdColaborador", colaboradorId)
                };


                var result = await _context.Database.ExecuteSqlRawAsync(
                    "exec DeleteColaborador @IdColaborador ;", parameters);
                 return result;
            } 
            catch (SqlException ex) when ( ex.Number >= 90001)
            {
                throw new KeyNotFoundException("El colaborador no existe.", ex);
            }
        }

        async Task<List<DefPuesto>> IColaboradorService.GetAllPuestoAsync()
        {
            
            var result = await _context.DefPuestos.FromSqlRaw("EXEC GetAllPuesto;").ToListAsync();
            return result;
        }

        async Task<List<Colaborador>> IColaboradorService.GetAllColaboradoresAsync()
        {
            
            var result = await _context.Colaboradores.FromSqlRaw("EXEC GetAllColaborador;").ToListAsync();
            return result;
        }

        async Task<int> IColaboradorService.UpdateColaboradorAsync(UpdateColaborador updColaborador)
        {
            try
            {
                var query = "exec UpdateColaborador @OldCodColaborador, @CodColaborador, @Nombre, @CodJefe, @IdPuesto";
                var parameters = new[]
                {
                    new SqlParameter("@OldCodColaborador", updColaborador.OldCodColaborador),
                    new SqlParameter("@CodColaborador", updColaborador.CodColaborador),
                    new SqlParameter("@Nombre", updColaborador.Nombre),
                    new SqlParameter("@CodJefe", updColaborador.CodJefe is null ||  updColaborador.CodJefe == -1 ? DBNull.Value : updColaborador.CodJefe ),
                    new SqlParameter("@IdPuesto", updColaborador.IdPuesto)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(query, parameters);
                return result;

            }
            catch (SqlException ex) when (ex.Number >= 90011 )
            {
                throw new KeyNotFoundException("Ha ocurrido un error: "+ ex.Message, ex);
            }
        }

        async Task<int> IColaboradorService.CreateColaboradorAsync(NewColaborador newColaborador)
        {
            try
            {
                var query = "exec CreateColaborador @CodColaborador , @Nombre , @CodJefe , @IdPuesto ";
                var parameters = new[]
                {
                    new SqlParameter("@CodColaborador", newColaborador.CodColaborador),
                    new SqlParameter("@Nombre", newColaborador.Nombre),
                    new SqlParameter("@CodJefe", newColaborador.CodJefe is null || newColaborador.CodJefe == -1 ?  DBNull.Value: newColaborador.CodJefe ),
                    new SqlParameter("@IdPuesto", newColaborador.IdPuesto)
                };

                var result = await _context.Database.ExecuteSqlRawAsync(query, parameters);
                return result;

            }
            catch (SqlException ex) when (ex.Number >= 90021)
            {
                throw new KeyNotFoundException("Ha ocurrido un error: " + ex.Message, ex);
            }
        }

    
    }
}
