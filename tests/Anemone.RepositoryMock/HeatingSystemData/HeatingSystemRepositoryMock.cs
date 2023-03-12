using Anemone.Repository.HeatingSystemData;
using Moq;

namespace Anemone.RepositoryMock.HeatingSystemData;

public class HeatingSystemRepositoryMock : IHeatingSystemRepository
{
    private List<HeatingSystem> _data = new();
    private Mock<IHeatingSystemRepository> _mock = new();
    private IHeatingSystemRepository Object => _mock.Object;
    private static DateTime CurrentDate => DateTime.UtcNow;

    public HeatingSystemRepositoryMock()
    {
        _mock.Setup(x => x.GetAllNames()).Returns(() =>
        {
            var names = _data.Select(x => new HeatingSystemName { Id = x.Id, Name = x.Name });
            return Task.FromResult(names);
        });
        
        _mock.Setup(x => x.Create(It.IsAny<HeatingSystem>()))
            .Callback<HeatingSystem>(heatingSystemModel =>
            {
                _data.Add(heatingSystemModel);
                PropertyHelper.GetSetterForProperty<HeatingSystem, int?>(x => x.Id)?.Invoke(heatingSystemModel, GenerateId());
                PropertyHelper.GetSetterForProperty<HeatingSystem, DateTime?>(x => x.CreationDate)?.Invoke(heatingSystemModel, CurrentDate);
                foreach (var point in heatingSystemModel.HeatingSystemPoints)
                {
                    PropertyHelper.GetSetterForProperty<HeatingSystemPoint, int?>(x => x.Id)?.Invoke(point, GenerateId());
                    PropertyHelper.GetSetterForProperty<HeatingSystemPoint, int?>(x => x.HeatingSystemId)?.Invoke(point, heatingSystemModel.Id);
                    PropertyHelper.GetSetterForProperty<HeatingSystemPoint, HeatingSystem?>(x => x.HeatingSystem)?.Invoke(point, heatingSystemModel);
                }
            });
        
        _mock.Setup(x => x.GetAll()).Returns(() => Task.FromResult(_data.AsEnumerable()));

        _mock.Setup(x => x.Update(It.IsAny<HeatingSystem>()))
            .Callback<HeatingSystem>(heatingSystemModel =>
            {
                if(_data.Contains(heatingSystemModel)) PropertyHelper.GetSetterForProperty<HeatingSystem, DateTime?>(x => x.ModificationDate)?.Invoke(heatingSystemModel, CurrentDate);
            });

        _mock.Setup(x => x.Get(It.IsAny<int>()))
            .Returns<int>(id =>
            {
                var model = _data.SingleOrDefault(x => x.Id == id);
                return Task.FromResult(model);
            });


        _mock.Setup(x => x.Delete(It.IsAny<HeatingSystem>()))
            .Callback<HeatingSystem>(x => _data.Remove(x));
    }


    private static readonly Random Random = new();

    private static int GenerateId()
    {
        return Random.Next();
    }

    public Task Create(HeatingSystem data)
    {
        Object.Create(data);
        return Task.CompletedTask;
    }

    public Task<HeatingSystem?> Get(int id)
    {
        return Object.Get(id);
    }

    public Task<IEnumerable<HeatingSystem>> GetAll()
    {
        return Object.GetAll();
    }

    public Task Update(HeatingSystem data)
    {
        return Object.Update(data);
    }

    public Task Delete(HeatingSystem data)
    {
        Object.Delete(data);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<HeatingSystemName>> GetAllNames()
    {
        return Object.GetAllNames();
    }
}