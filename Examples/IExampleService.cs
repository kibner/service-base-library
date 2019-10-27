using System.Collections.Generic;
using ServiceBaseLibrary;

namespace Examples
{
    public interface IExampleService : IService<ExampleClass>
    {
        List<ExampleClass> GetManyByNavigationClassId(int navigationClassId);
        Page<ExampleClass> GetPageByNavigationClassId(int navigationClassId);
    }
}