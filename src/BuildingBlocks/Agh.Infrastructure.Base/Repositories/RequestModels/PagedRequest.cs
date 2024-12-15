namespace Agh.Infrastructure.Base.Repositories.RequestModels;

public class PagedRequest
{
    public int Page { get; set; } = 1; // Hangi sayfa indexi dönecek
    public int PageSize { get; set; } = 25; // Her sayfada kaç eleman olacak
    public string? OrderBy { get; set; } // Sıralama kolon adı
    public bool OrderByDescending { get; set; } // Azalan sıralama mı?
    public string? SearchQuery { get; set; } // Genel bir arama sorgusu
    public Dictionary<string, string>? Filters { get; set; } // İsteğe bağlı filtreler (Key=Column, Value=FilterValue)
}