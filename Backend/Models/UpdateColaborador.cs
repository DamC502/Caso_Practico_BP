namespace Backend.Models
{
    public class UpdateColaborador
    {
        public long OldCodColaborador { get; set; }
        public long CodColaborador { get; set; }
        public string Nombre { get; set; }
        public long? CodJefe { get; set; }
        public long IdPuesto { get; set; }
    }
}
