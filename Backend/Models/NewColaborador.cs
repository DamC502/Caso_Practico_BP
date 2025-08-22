namespace Backend.Models
{
    public class NewColaborador
    {
        public long CodColaborador { get; set; }
        public string Nombre { get; set; }
        public long? CodJefe { get; set; }
        public long IdPuesto { get; set; }
    }
}
