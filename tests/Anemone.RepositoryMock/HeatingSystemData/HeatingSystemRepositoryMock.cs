using Anemone.Repository.HeatingSystemData;
using Moq;

namespace Anemone.RepositoryMock.HeatingSystemData;

public class HeatingSystemRepositoryMock : Mock<IHeatingSystemRepository>
{
    private readonly List<HeatingSystem> _data = new();
    private readonly Random _random = new();

    public HeatingSystemRepositoryMock()
    {
        InitializeMethodsSetup();
    }

    private static DateTime CurrentDate => DateTime.UtcNow;

    private void InitializeMethodsSetup()
    {
        Setup(m => m.GetAllNames()).Returns(GetNamesFromCollection);
        Setup(m => m.Exists(It.IsAny<int>())).Returns(CollectionItemExists);
        Setup(m => m.Get(It.IsAny<int>())).Returns<int>(GetCollectionItem);
        Setup(m => m.GetAll()).Returns(GetCollectionItems);
        Setup(m => m.Create(It.IsAny<HeatingSystem>())).Callback<HeatingSystem>(AddHeatingSystemToCollection);
        Setup(m => m.Update(It.IsAny<HeatingSystem>())).Callback<HeatingSystem>(UpdateCollectionItem);
        Setup(m => m.Delete(It.IsAny<HeatingSystem>())).Callback<HeatingSystem>(DeleteCollectionItem);
    }

    private Task<IEnumerable<HeatingSystemName>> GetNamesFromCollection()
    {
        var names = _data.Select(x => new HeatingSystemName { Id = x.Id, Name = x.Name });
        return Task.FromResult(names);
    }

    private Task<bool> CollectionItemExists(int id)
    {
        return Task.FromResult(_data.Exists(x => x.Id == id));
    }


    private Task<HeatingSystem?> GetCollectionItem(int id)
    {
        var model = _data.SingleOrDefault(x => x.Id == id);
        return Task.FromResult(model);
    }


    private Task<IEnumerable<HeatingSystem>> GetCollectionItems()
    {
        return Task.FromResult(_data.AsEnumerable());
    }


    private void AddHeatingSystemToCollection(HeatingSystem entity)
    {
        _data.Add(entity);
        SetDbEntityProperties(entity);
        SetDbEntityProperties(entity, entity.HeatingSystemPoints);
    }


    private void SetDbEntityProperties(HeatingSystem entity)
    {
        PropertyRetriever.GetSetterForProperty<HeatingSystem, int?>(x => x.Id).Invoke(entity, GenerateId());
        PropertyRetriever.GetSetterForProperty<HeatingSystem, DateTime?>(x => x.CreationDate)
            .Invoke(entity, CurrentDate);
    }

    private void SetDbEntityProperties(HeatingSystem entity, IEnumerable<HeatingSystemPoint> points)
    {
        foreach (var point in points) SetDbPointProperties(entity, point);
    }

    private void SetDbPointProperties(HeatingSystem entity, HeatingSystemPoint point)
    {
        PropertyRetriever.GetSetterForProperty<HeatingSystemPoint, int?>(x => x.Id).Invoke(point, GenerateId());
        PropertyRetriever.GetSetterForProperty<HeatingSystemPoint, int?>(x => x.HeatingSystemId)
            .Invoke(point, entity.Id);
        PropertyRetriever.GetSetterForProperty<HeatingSystemPoint, HeatingSystem?>(x => x.HeatingSystem)
            .Invoke(point, entity);
    }

    private int GenerateId()
    {
        return _random.Next();
    }

    private void UpdateCollectionItem(HeatingSystem entity)
    {
        if (_data.Contains(entity))
            PropertyRetriever.GetSetterForProperty<HeatingSystem, DateTime?>(x => x.ModificationDate)
                .Invoke(entity, CurrentDate);
    }

    private void DeleteCollectionItem(HeatingSystem entity)
    {
        _data.Remove(entity);
    }


    public HeatingSystem CreateObjectInRepository()
    {
        var heatingSystem = CreateHeatingSystem();
        Object.Create(heatingSystem);
        return heatingSystem;
    }

    public HeatingSystem[] CreateObjectInRepository(int count)
    {
        var output = new List<HeatingSystem>();
        for (var i = 0; i <= count; i++) output.Add(CreateObjectInRepository());

        return output.ToArray();
    }

    private static HeatingSystem CreateHeatingSystem()
    {
        return HeatingSystemFaker.GenerateHeatingSystem();
    }
}