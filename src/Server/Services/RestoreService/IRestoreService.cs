using Models;

namespace Services.RestoreService;

public interface IRestoreService
{
    void GetRestoration();

    Task SyncData(SystemNode node, RestoreType type);
}