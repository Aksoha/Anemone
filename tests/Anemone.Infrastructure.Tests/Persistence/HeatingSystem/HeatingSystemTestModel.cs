namespace Anemone.Infrastructure.Tests.Persistence.HeatingSystem;

public class HeatingSystemTestModel : Core.Common.Entities.HeatingSystem
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