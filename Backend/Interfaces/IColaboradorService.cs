using Backend.Models;

namespace Backend.Interfaces
{
    public interface IColaboradorService
    {

        Task<List<Colaborador>> GetAllColaboradoresAsync();

        Task<List<DefPuesto>> GetAllPuestoAsync(); 

        Task<int> DeleteColaboradorAsync(long colaboradorId);
        Task<int> UpdateColaboradorAsync(UpdateColaborador updColaborador);

        Task<int> CreateColaboradorAsync(NewColaborador newColaborador);




    }
}
