using Anemone.Core.Common.Entities;

namespace Anemone.Infrastructure.Tests.Persistence.HeatingSystem;

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

    public new Core.Common.Entities.HeatingSystem? HeatingSystem
    {
        get => base.HeatingSystem;
        set => base.HeatingSystem = value;
    }
}