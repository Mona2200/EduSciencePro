namespace EduSciencePro.ViewModels.Response;

/// <summary>
/// Модель представления для главной страницы.
/// </summary>
public class MainPageViewModel
{
    /// <summary>
    /// Новости.
    /// </summary>
    public List<PostViewModel> LastNews { get; set; }

    /// <summary>
    /// Обсуждения.
    /// </summary>
    public List<PostViewModel> LastDiscuss { get; set; }

    /// <summary>
    /// Конференции.
    /// </summary>
    public List<ConferenceViewModel> LastConferences { get; set; }
}
