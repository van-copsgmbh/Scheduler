namespace Corima.Services
{
    public class MyService
    {
        private RepositoryService service;
        public MyService(RepositoryService repositoryService)
        {
            service = repositoryService;
        }

        public void Save()
        {
            service.Save("MyService data");
        }
    }
}