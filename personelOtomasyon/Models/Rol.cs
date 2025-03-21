using System.ComponentModel.DataAnnotations;

namespace personelOtomasyon.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        public string RolAdi { get; set; }
    }
}
