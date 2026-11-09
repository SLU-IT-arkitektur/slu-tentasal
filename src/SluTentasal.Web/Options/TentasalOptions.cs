namespace SluTentasal.Web.Options;

public sealed class TentasalOptions
{
    public const string SectionName = "Tentasal";

    /// <summary>Hur många minuter före tentastart lösenordet börjar visas.</summary>
    public int ShowBeforeStartMinutes { get; set; } = 30;

    /// <summary>Hur många minuter efter tentastart lösenordet slutar visas.</summary>
    public int ShowAfterStartMinutes { get; set; } = 45;

    /// <summary>Hur ofta (sekunder) sidan hämtar lösenordet på nytt.</summary>
    public int PasswordPollSeconds { get; set; } = 60;

    /// <summary>Tidszon som starttime i databasen är lagrad i.</summary>
    public string TimeZone { get; set; } = "Europe/Stockholm";
}
