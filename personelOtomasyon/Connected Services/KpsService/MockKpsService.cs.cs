namespace personelOtomasyon.Connected_Services.KpsService
{
    public class MockKpsService:IKpsService
    {
        public Task<bool> TcKimlikDogrulaAsync(long tcNo, string ad, string soyad, int dogumYili)
        {
            // Herkesi doğrula (test ortamı)
            return Task.FromResult(true);

        }
    }
}
