namespace Anemone.Repository.Tests;

public class HeatingSystemTestModel : HeatingSystem
{
    public new int? Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    public new DateTime? CreationDate
    {
        get => base.CreationDate;
        set => base.CreationDate = value;
    }
}