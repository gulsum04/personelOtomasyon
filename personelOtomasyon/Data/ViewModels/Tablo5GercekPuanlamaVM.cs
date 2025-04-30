using personelOtomasyon.Models;

namespace personelOtomasyon.Data.ViewModels
{
    public class Tablo5GercekPuanlamaVM
    {
        public string IlanBaslik { get; set; }
        public string AdayAdSoyad { get; set; }
        public int ToplamPuan { get; set; }
        public List<BasvuruPuan> Puanlar { get; set; }
    }
}
