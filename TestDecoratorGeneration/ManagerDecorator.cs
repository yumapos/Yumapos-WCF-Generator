namespace TestDecoratorGeneration
{
    public partial class ManagerDecorator
    {
        private TestDecoratorGeneration.Manager DecoratedComponent;

        public ManagerDecorator()
        {
            DecoratedComponent = new Manager();
        }
    }
}