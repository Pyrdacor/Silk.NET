using System;
using System.Reflection;

namespace Silk.NET.UI.Common
{
    public class TemplateAttribute : Attribute
    {
        internal Type TemplateType { get; }

        public TemplateAttribute(Type templateType)
        {
            TemplateType = templateType;
        }
    }

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

        internal static Component Create(Type type, string id)
        {
            var templateAttributes = type.GetCustomAttributes(typeof(TemplateAttribute), false);

            if (templateAttributes.Length == 0)
                throw new InvalidOperationException("Components need the attribute `Template`");

            var stylesAttributes = type.GetCustomAttributes(typeof(StylesAttribute), false);

            if (stylesAttributes.Length == 0)
                throw new InvalidOperationException("Components need the attribute `Styles`");

            var templateType = (templateAttributes[0] as TemplateAttribute).TemplateType;
            var stylesType = (stylesAttributes[0] as StylesAttribute).StylesType;

            if (!templateType.IsSubclassOf(typeof(Template)))
                throw new InvalidOperationException($"Component template type {templateType.Name} is not derived from class `Template`");
            if (!stylesType.IsSubclassOf(typeof(Styles)))
                throw new InvalidOperationException($"Component styles type {stylesType.Name} is not derived from class `Styles`");

            try
            {
                var component = Activator.CreateInstance(type) as Component;

                if (component == null)
                    throw new InvalidOperationException($"Type {type.Name} is no valid component.");

                component.Id = id;
                component.template = (Template)Activator.CreateInstance(templateType);
                component.styles = (Styles)Activator.CreateInstance(stylesType);
                
                return component;
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
