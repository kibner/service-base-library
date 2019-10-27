using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examples
{
    public class ExampleClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int NavigationClassId { get; set; }
        public string Name { get; set; }

        public virtual NavigationClass NavigationClass { get; set; }
    }
}