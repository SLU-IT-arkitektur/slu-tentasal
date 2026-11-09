using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SluTentasal.Web.Data;
using SluTentasal.Web.Options;

namespace SluTentasal.Web.Pages;

public class IndexModel(PasswordRepository repository, LocalClock clock, IOptions<TentasalOptions> options) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Campus { get; set; }

    public int PollSeconds => options.Value.PasswordPollSeconds;

    public void OnGet()
    {
    }

    public PartialViewResult OnGetClock() => Partial("_Clock", clock.Now);

    public async Task<IActionResult> OnGetPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(Campus))
        {
            return BadRequest();
        }

        var now = clock.Now;
        var active = await repository.GetActiveAsync(Campus, now);
        var next = active is null ? await repository.GetNextStartAsync(Campus, now) : null;

        return Partial("_Password", new PasswordViewModel(
            Campus,
            now,
            active,
            next,
            options.Value.ShowBeforeStartMinutes,
            options.Value.ShowAfterStartMinutes));
    }
}

public sealed record PasswordViewModel(
    string Campus,
    DateTime Now,
    ActivePassword? Active,
    DateTime? NextStart,
    int ShowBeforeStartMinutes,
    int ShowAfterStartMinutes);
