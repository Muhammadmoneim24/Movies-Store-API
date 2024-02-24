using System.ComponentModel.DataAnnotations;

namespace APIs_Trainee.dtos
{
    public class CreateGenredto
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
