namespace DatingApp.Application.Execptions
{
    public class NotFoundException :Exception
    {
        public NotFoundException( string entityName ,object key):base($"entity name {entityName} " +
            $"with id {key}  not found")
        {
            
        }
    }
}
