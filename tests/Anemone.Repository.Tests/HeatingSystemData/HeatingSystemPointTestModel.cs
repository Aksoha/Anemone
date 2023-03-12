namespace Anemone.Repository.Tests.HeatingSystemData;

public class HeatingSystemPointTestModel : HeatingSystemPoint
{
    public new int? Id
    {
        get => base.Id;
        set => base.Id = value;
    }

    public new int? HeatingSystemId
    {
        get => base.HeatingSystemId;
        set => base.HeatingSystemId = value;
    }

    public new HeatingSystem? HeatingSystem
    {
        get => base.HeatingSystem;
        set => base.HeatingSystem = value;
    }
}