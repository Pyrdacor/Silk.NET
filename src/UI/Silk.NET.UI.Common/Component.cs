using System;
using System.Reflection;

namespace Silk.NET.UI.Common
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TemplateAttribute : Attribute
    {
        internal Type TemplateType { get; }

        public TemplateAttribute(Type templateType)
        {
            TemplateType = templateType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class StylesAttribute : Attribute
    {
        internal Type StylesType { get; }

        public StylesAttribute(Type stylesType)
        {
            StylesType = stylesType;
        }
    }

    public abstract class Component : Control
    {
        private Template template;
        private Styles styles;

        protected Component()
            : base(null)
        {

        }

        internal override void InitView()
        {
            template.CreateFor(this);
            styles.Apply(template, this);
        }

        internal void DestroyView()
        {
            // TODO ?
        }

        internal static Component Create(Type type, string id, bool root)
        {
            var templateAttribute = type.GetCustomAttribute(typeof(TemplateAttribute), false);

            if (templateAttribute == null)
                throw new InvalidOperationException($"Component `{type.Name}` needs the attribute `Template`.");

            var stylesAttribute = type.GetCustomAttribute(typeof(StylesAttribute), false);

            if (stylesAttribute == null)
                throw new InvalidOperationException($"Component `{type.Name}` needs the attribute `Styles`.");

            var templateType = (templateAttribute as TemplateAttribute).TemplateType;
            var stylesType = (stylesAttribute as StylesAttribute).StylesType;

            if (!templateType.IsSubclassOf(typeof(Template)))
                throw new InvalidOperationException($"Component template type {templateType.Name} is not derived from class `Template`.");
            if (!stylesType.IsSubclassOf(typeof(Styles)))
                throw new InvalidOperationException($"Component styles type {stylesType.Name} is not derived from class `Styles`.");

            Component component = root ? TryTypeCreation<RootComponent>(type) : TryTypeCreation<Component>(type);

            if (component == null)
                throw new InvalidOperationException($"Type {type.Name} is not derived from class `Component`.");

            component.Id = id;
            component.template = TryTypeCreation<Template>(templateType);
            component.styles = TryTypeCreation<Styles>(stylesType);

            return component;
        }

        private static T TryTypeCreation<T>(Type type) where T : class
        {
            try
            {
                return (T)Activator.CreateInstance(type);
            }
            catch (Exception ex)            
            {                
                if (ex is MissingMemberException || ex is MissingMethodException ||
                    ex is MemberAccessException || ex is MethodAccessException)
                {
                    throw new InvalidOperationException($"There is no public parameterless constructor in type {type.Name}.");
                }
                else if (ex is TargetInvocationException)
                {
                    throw new InvalidOperationException($"Constructor of type {type.Name} threw an exception. See inner exception for details.", ex);
                }

                throw;
            }
        }
    }
}
