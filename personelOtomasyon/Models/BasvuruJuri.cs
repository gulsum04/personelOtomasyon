namespace personelOtomasyon.Models
{
    public class BasvuruJuri
    {
        public int Id { get; set; }

        public int BasvuruId { get; set; }
        public Basvuru Basvuru { get; set; }

        public string JuriId { get; set; }
        public ApplicationUser Juri { get; set; }
    }
}
