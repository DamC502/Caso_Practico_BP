namespace Backend.Models
{
    public class Colaborador
    {
        public long Codigo_Colaborador { get; set; }
        public string Colaborador_Nombre { get; set; }
        public int Nivel { get; set;  }
        public long? Codigo_Jefe { get; set; }
        public string Puesto { get; set; }

        public long Id_puesto { get; set; }
    }
}
