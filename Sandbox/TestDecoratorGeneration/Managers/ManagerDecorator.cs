using System.Threading.Tasks;

namespace TestDecoratorGeneration.Managers
{
    public partial class ManagerDecorator
    {
        public ManagerDecorator()
        {
            DecoratedComponent = new Manager();
        }

        private async Task OnEntryAsync(string start, object[] objects)
        {
            OnEntry(start, objects);
        }
        private void OnEntry(string start, object[] objects)
        {
        }
    }
}