namespace Silk.NET.UI.Common.Dynamic
{
    internal class ConditionalComponentBinder : ComponentBinder
    {
        private Component boundComponent;
        private Observable<bool> condition;
        private string componentTypeName;
        private string componentId;

        public ConditionalComponentBinder(Observable<bool> condition, string componentTypeName, string componentId)
        {
            this.condition = condition;
            this.componentTypeName = componentTypeName;
            this.componentId = componentId;
        }

        public Observable<bool> GetElseObservable()
        {
            return condition.Map(c => !c);
        }

        public override void Bind(Component parentComponent)
        {
            condition.Subscribe(result =>
            {
                if (result)
                {
                    if (boundComponent == null)
                        boundComponent = ComponentManager.InitializeComponent(componentTypeName, componentId);
                    
                    boundComponent.AddTo(parentComponent);
                }
                else
                {
                    if (boundComponent != null)
                    {
                        boundComponent.Destroy();
                        boundComponent = null;
                    }
                }
            });
        }
    }
}