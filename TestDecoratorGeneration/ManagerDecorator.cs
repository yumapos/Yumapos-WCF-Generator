using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public partial class ManagerDecorator
    {
        public ManagerDecorator()
        {
            DecoratedComponent = new Manager();
        }

        private Task InitAsync(string start, object[] objects)
        {
            throw new System.NotImplementedException();
        }
    }
}