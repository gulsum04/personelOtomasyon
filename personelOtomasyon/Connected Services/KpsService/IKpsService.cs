namespace personelOtomasyon.Connected_Services.KpsService
{
    public interface IKpsService
    {
        Task<bool> TcKimlikDogrulaAsync(long tcNo, string ad, string soyad, int dogumYili);
    }
}
