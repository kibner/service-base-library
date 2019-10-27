using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using ServiceBaseLibrary;

namespace Examples
{
    public class ExampleService : ServiceBase<ExampleClass>, IExampleService
    {
        public ExampleService(DbContext context) : base(context)
        {
        }

        public List<ExampleClass> GetManyByNavigationClassId(int navigationClassId)
        {
            var orderBy = new List<IOrderBy<ExampleClass>>
            {
                new OrderBy<ExampleClass>(exampleClass => exampleClass.Name, OrderDirection.Ascending)
            };

            var includes = new List<Expression<Func<ExampleClass, dynamic>>>
            {
                exampleClass => exampleClass.NavigationClass
            };

            var result = GetMany(exampleClass => exampleClass.NavigationClassId == navigationClassId, orderBy,
                includes);

            return result;
        }

        public Page<ExampleClass> GetPageByNavigationClassId(int navigationClassId)
        {
            var page = new Page<ExampleClass>
            {
                PageNumber = 3,
                PageSize = 15
            };

            var orderBy = new List<IOrderBy<ExampleClass>>
            {
                new OrderBy<ExampleClass>(exampleClass => exampleClass.Name, OrderDirection.Ascending)
            };

            var result = GetManyPaged(page, exampleClass => exampleClass.NavigationClassId == navigationClassId,
                orderBy);

            return result;
        }
    }
}