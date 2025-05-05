using personelOtomasyon.Models;
using System.Collections.Generic;

    namespace personelOtomasyon.Data.ViewModels
    {
        public class Tablo5PuanlamaVM
        {
        public string IlanBaslik { get; set; }
         public string AdayAdSoyad { get; set; }
         public int ToplamPuan { get; set; }
         public List<KadroKriterAlt> AltKriterler { get; set; }
         public Dictionary<string, int> PuanSozluk { get; set; }
     

        }

    }


