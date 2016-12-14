namespace TestDecoratorGeneration
{
    public partial class ManagerDecorator
    {
        public ManagerDecorator()
        {
            DecoratedComponent = new Manager();
        }
    }
}