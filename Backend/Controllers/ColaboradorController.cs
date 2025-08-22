using Backend.Interfaces;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    /// <summary>
    ///  Controlador para CRUD de colaboradores
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class ColaboradorController : Controller
    {

        private readonly IColaboradorService _colaboradorServiceService;

        public ColaboradorController(IColaboradorService colaboradorServiceService)
        {
            _colaboradorServiceService = colaboradorServiceService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Colaborador>>> GetColaboradores()
        {
            try
            {
                var colaboradores = await _colaboradorServiceService.GetAllColaboradoresAsync();
                return Ok(colaboradores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", detail = ex.Message });
            }
        }


        [HttpGet("GetPuestos")]
        public async Task<ActionResult<List<DefPuesto>>> GetPuestos()
        {
            try
            {
                var puestos = await _colaboradorServiceService.GetAllPuestoAsync();
                return Ok(puestos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                await _colaboradorServiceService.DeleteColaboradorAsync(id);
                return Ok(new { message = $"Colaborador {id} inactivado correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateColaborador([FromBody] UpdateColaborador updcolaborador)
        {
            if (updcolaborador == null)
            {
                return BadRequest(new { error = "El cuerpo de la solicitud no puede estar vacío." });
            }

            if (updcolaborador.OldCodColaborador <= 0 ||
                updcolaborador.CodColaborador <= 0 || 
                updcolaborador.IdPuesto <= 0 ||
                updcolaborador.CodJefe < -1 ||
                string.IsNullOrEmpty(updcolaborador.Nombre))
            {
                return BadRequest(new {error = "Los datos del colaborador son inválidos." });
            }

            
            try
            {
                var filasAfectadas = await _colaboradorServiceService.UpdateColaboradorAsync(updcolaborador);
                if (filasAfectadas > 0)
                {
                    return Ok($"Colaborador actualizado correctamente.");
                }
                return NotFound( new {error = "No se encontró el colaborador o no se realizó ninguna actualización." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateColaborador([FromBody] NewColaborador newColaborador)
        {
            if (newColaborador == null)
            {
                return BadRequest(new { error = "El cuerpo de la solicitud no puede estar vacío." });
            }

            if (newColaborador.CodColaborador <= 0 ||
                newColaborador.IdPuesto <= 0 ||
                newColaborador.CodJefe < -1 ||
                string.IsNullOrEmpty(newColaborador.Nombre))
            {
                return BadRequest(new { error = "Los datos del colaborador son inválidos." });
            }


            try
            {
                var filasAfectadas = await _colaboradorServiceService.CreateColaboradorAsync(newColaborador);
                if (filasAfectadas > 0)
                {
                    return Ok($"Colaborador creado correctamente.");
                }
                return NotFound("No se guardo el o no se realizó ninguna acción.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor.", detail = ex.Message });
            }
        }

    }
}
