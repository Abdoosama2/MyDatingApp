using System.ComponentModel.DataAnnotations;

namespace DatingApp.Application.DTO.Hubs
{
    public class Group(string name)
    {
        [Key]
        public  string Name { get; set; } = name;

        public ICollection<Connection> Connections { get; set; } = [];


    }
}
