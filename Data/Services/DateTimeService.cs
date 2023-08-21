namespace EduSciencePro.Data.Services;

public class DateTimeService
{
    public string GetDate(DateOnly date)
    {
        var month = $"{date.Month}";
        if (month.Length == 1)
            month = $"0{month}";
        var day = $"{date.Day}";
        if (day.Length == 1)
            day = $"0{day}";

            return $"{day}.{month}.{date.Year}";
    }

    public string GetDate(DateTime date)
    {
        var month = $"{date.Month}";
        if (month.Length == 1)
            month = $"0{month}";
        var day = $"{date.Day}";
        if (day.Length == 1)
            day = $"0{day}";

        return $"{day}.{month}.{date.Year}";
    }
}
