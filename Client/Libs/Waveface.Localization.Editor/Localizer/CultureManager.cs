#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace Waveface.Localization
{
    // Defines a component for managing the User Interface culture for
    // a form (or control) and allows the UICulture of an individual form 
    // or entire application be changed dynamically.
    // This handles forms and components developed in C# and VB.NET but may or may not 
    // work for components/forms developed in other languages depending on how the language 
    // handles resource naming and serialization.
    [ToolboxItem(true)]
    public class CultureManager : Component
    {
        // Represents the method that will handle the UICultureChanged event   
        public delegate void CultureChangedHandler(CultureInfo newCulture);

        // Raised when the ApplicationUICulture is changed  
        public static event CultureChangedHandler ApplicationUICultureChanged;

        #region Field

        private const AnchorStyles AnchorLeftRight = AnchorStyles.Left | AnchorStyles.Right;
        private const AnchorStyles AnchorTopBottom = AnchorStyles.Top | AnchorStyles.Bottom;
        private const AnchorStyles AnchorAll = AnchorLeftRight | AnchorTopBottom;

        // The application UI culture
        private static CultureInfo s_applicationUiCulture;

        // Should the Thread.CurrentCulture be changed when the UICulture changes.
        private static bool s_synchronizeThreadCulture = true;

        // Properties to be excluded when applying culture resources
        private List<string> m_excludeProperties = new List<string>();

        // The control (or more usually form) being managed
        private Control m_managedControl;

        // If true form location is preserved when changing culture
        private bool m_preserveFormLocation = true;

        // If true form size is preserved when changing culture
        private bool m_preserveFormSize = true;

        // Should the Form UICulture be changed when the ApplicationUICulture changes.
        private bool m_synchronizeUiCulture = true;

        // The current UI culture for the managed control
        private CultureInfo m_uiCulture;

        #endregion

        #region Public Static Methods

        // Set/Get the UICulture for whole application. 
        // Setting this property changes the Thread.CurrentUICulture 
        // and sets the UICulture for all CultureManagers to the given culture.  
        public static CultureInfo ApplicationUICulture
        {
            get
            {
                if (s_applicationUiCulture == null)
                {
                    s_applicationUiCulture = Thread.CurrentThread.CurrentUICulture;
                }

                return s_applicationUiCulture;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (!value.Equals(s_applicationUiCulture))
                {
                    s_applicationUiCulture = value;
                    SetThreadUICulture(SynchronizeThreadCulture);

                    if (ApplicationUICultureChanged != null)
                    {
                        ApplicationUICultureChanged(value);
                    }
                }
            }
        }

        // If set to true then the Thread.CurrentCulture property is changed
        // to match the current UICulture
        public static bool SynchronizeThreadCulture
        {
            get { return s_synchronizeThreadCulture; }
            set
            {
                s_synchronizeThreadCulture = value;

                if (value)
                {
                    SetThreadUICulture(true);
                }
            }
        }

        public static void SetThreadUICulture(bool synchronizeThreadCulture)
        {
            Thread.CurrentThread.CurrentUICulture = ApplicationUICulture;

            if (synchronizeThreadCulture)
            {
                if (ApplicationUICulture.IsNeutralCulture)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ApplicationUICulture.Name);
                }
                else
                {
                    Thread.CurrentThread.CurrentCulture = ApplicationUICulture;
                }
            }
        }

        #endregion

        #region Public Instance Methods

        // Create a new instance of the component
        public CultureManager()
        {
            ApplicationUICultureChanged += OnApplicationUICultureChanged;
        }

        // Create a new instance of the component
        public CultureManager(IContainer container)
            : this()
        {
            container.Add(this);
        }

        // The control or form to manage the UICulture for
        [Description("The control or form to manage the UICulture for")]
        public Control ManagedControl
        {
            get
            {
                if (m_managedControl == null)
                {
                    if (Site != null)
                    {
                        IDesignerHost _host = Site.GetService(typeof (IDesignerHost)) as IDesignerHost;

                        if ((_host != null) && (_host.Container != null) && (_host.Container.Components.Count > 0))
                        {
                            m_managedControl = _host.Container.Components[0] as Control;
                        }
                    }
                }

                return m_managedControl;
            }
            set { m_managedControl = value; }
        }

        // Should the form size be preserved when the culture is changed
        [DefaultValue(true)]
        [Description("Should the form size be preserved when the culture is changed")]
        public bool PreserveFormSize
        {
            get { return m_preserveFormSize; }
            set { m_preserveFormSize = value; }
        }

        // Should the form location be preserved when the culture is changed
        [DefaultValue(true)]
        [Description("Should the form location be preserved when the culture is changed")]
        public bool PreserveFormLocation
        {
            get { return m_preserveFormLocation; }
            set { m_preserveFormLocation = value; }
        }

        // List of properties to exclude when applying culture specific resources
        [Editor(
            "System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
            , typeof (UITypeEditor))]
        [Description("List of properties to exclude when applying culture specific resources")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> ExcludeProperties
        {
            get { return m_excludeProperties; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                m_excludeProperties = value;
            }
        }

        // The current user interface culture for the ManagedControl. 
        // This can be set independently of the ApplicationUICulture, allowing
        // you to have an application simultaneously displaying forms with different cultures.   
        // Set the ApplicationUICulture to change the UICulture of
        // all forms in the application which have associated CultureManagers.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CultureInfo UICulture
        {
            get
            {
                if (m_uiCulture == null)
                {
                    m_uiCulture = ApplicationUICulture;
                }

                return m_uiCulture;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                ChangeUICulture(value);
            }
        }

        [DefaultValue(true)]
        [Description("Should the UICulture of this form be changed when the ApplicationUICulture")]
        public bool SynchronizeUICulture
        {
            get { return m_synchronizeUiCulture; }
            set { m_synchronizeUiCulture = value; }
        }

        // Raised when the UICulture is changed for this component.  This enables forms
        // to update their layout following a change to the UICulture.
        public event CultureChangedHandler UICultureChanged;

        // Called by framework to determine whether the ExcludeProperties should be serialised
        private bool ShouldSerializeExcludeProperties()
        {
            return (m_excludeProperties.Count > 0);
        }

        // Called by framework to reset the ExcludeProperties
        private void ResetExcludeProperties()
        {
            m_excludeProperties.Clear();
        }

        #endregion

        #region Local Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // detach from the global event handler
                ApplicationUICultureChanged -= OnApplicationUICultureChanged;
            }

            base.Dispose(disposing);
        }

        protected virtual void OnApplicationUICultureChanged(CultureInfo newCulture)
        {
            if (SynchronizeUICulture)
            {
                ChangeUICulture(newCulture);
            }
        }

        protected virtual void OnUICultureChanged(CultureInfo newCulture)
        {
            if (UICultureChanged != null)
            {
                UICultureChanged(newCulture);
            }
        }

        // Set the UI Culture for the managed form/control
        protected virtual void ChangeUICulture(CultureInfo culture)
        {
            if (!culture.Equals(m_uiCulture))
            {
                m_uiCulture = culture;

                if (m_managedControl != null)
                {
                    m_managedControl.SuspendLayout();

                    foreach (Control _childControl in m_managedControl.Controls)
                        _childControl.SuspendLayout();

                    try
                    {
                        ApplyResources(m_managedControl.GetType(), m_managedControl, culture);
                        OnUICultureChanged(culture);
                    }
                    finally
                    {
                        foreach (Control _childControl in m_managedControl.Controls)
                            _childControl.ResumeLayout();

                        m_managedControl.ResumeLayout();
                    }
                }
            }
        }

        /*
        private static void SetThreadCulture(CultureInfo value)
        {
            if (value.IsNeutralCulture)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(value.Name);
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = value;
            }
        }
        */

        // Load the localized resource for the given culture into a sorted list (indexed by resource name)
        private void LoadResources(ComponentResourceManager rm, CultureInfo culture,
                                   SortedList<string, object> resources)
        {
            if (!culture.Equals(CultureInfo.InvariantCulture))
            {
                LoadResources(rm, culture.Parent, resources);
            }

            ResourceSet _resourceSet = rm.GetResourceSet(culture, true, true);

            if (_resourceSet != null)
            {
                foreach (DictionaryEntry _entry in _resourceSet)
                    resources[(string) _entry.Key] = _entry.Value;
            }
        }

        // Set the size of a control handling docked/anchored controls appropriately
        protected virtual void SetControlSize(Control control, Size size)
        {
            // if the control is a form and we are preserving form size then exit
            if (control is Form && PreserveFormSize)
                return;

            // if dock fill or anchor all is set then don't change the size
            if (control.Dock == DockStyle.Fill || control.Anchor == AnchorAll)
                return;

            // if docked top/bottom or anchored left/right don't change the width
            if (control.Dock == DockStyle.Top || control.Dock == DockStyle.Bottom ||
                (control.Anchor & AnchorLeftRight) == AnchorLeftRight)
            {
                size.Width = control.Width;
            }

            // if docked left/right or anchored top/bottom don't change the height
            if (control.Dock == DockStyle.Left || control.Dock == DockStyle.Right ||
                (control.Anchor & AnchorTopBottom) == AnchorTopBottom)
            {
                size.Height = control.Height;
            }

            control.Size = size;
        }

        // Set the location of a control handling docked/anchored controls appropriately
        protected virtual void SetControlLocation(Control control, Point location)
        {
            // if the control is a form and we are preserving form location then exit
            if (control is Form && PreserveFormLocation)
                return;

            // if dock is set then don't change the location
            if (control.Dock != DockStyle.None)
                return;

            // if anchored to the right (but not left) then don't change x coord
            if ((control.Anchor & AnchorLeftRight) == AnchorStyles.Right)
            {
                location.X = control.Left;
            }

            // if anchored to the bottom (but not top) then don't change y coord
            if ((control.Anchor & AnchorTopBottom) == AnchorStyles.Bottom)
            {
                location.Y = control.Top;
            }

            control.Location = location;
        }

        // Apply a resource for an extender provider to the given control
        protected virtual void ApplyExtenderResource(Dictionary<Type, IExtenderProvider> extenderProviders,
                                                     Control control, string propertyName, object value)
        {
            IExtenderProvider _extender;

            switch (propertyName)
            {
                case "ToolTip":
                    if (extenderProviders.TryGetValue(typeof (ToolTip), out _extender))
                    {
                        (_extender as ToolTip).SetToolTip(control, value as string);
                    }

                    break;

                case "HelpKeyword":
                    if (extenderProviders.TryGetValue(typeof (HelpProvider), out _extender))
                    {
                        (_extender as HelpProvider).SetHelpKeyword(control, value as string);
                    }

                    break;

                case "HelpString":
                    if (extenderProviders.TryGetValue(typeof (HelpProvider), out _extender))
                    {
                        (_extender as HelpProvider).SetHelpString(control, value as string);
                    }

                    break;

                case "ShowHelp":
                    if (extenderProviders.TryGetValue(typeof (HelpProvider), out _extender))
                    {
                        (_extender as HelpProvider).SetShowHelp(control, (bool) value);
                    }

                    break;

                case "Error":
                    if (extenderProviders.TryGetValue(typeof (ErrorProvider), out _extender))
                    {
                        (_extender as ErrorProvider).SetError(control, value as string);
                    }

                    break;

                case "IconAlignment":
                    if (extenderProviders.TryGetValue(typeof (ErrorProvider), out _extender))
                    {
                        (_extender as ErrorProvider).SetIconAlignment(control, (ErrorIconAlignment) value);
                    }

                    break;

                case "IconPadding":
                    if (extenderProviders.TryGetValue(typeof (ErrorProvider), out _extender))
                    {
                        (_extender as ErrorProvider).SetIconPadding(control, (int) value);
                    }

                    break;
            }
        }

        // Return true if the given assembly was compiled using VB.NET
        protected static bool IsVBAssembly(Assembly assembly)
        {
            AssemblyName[] _referencedAssemblies = assembly.GetReferencedAssemblies();

            foreach (AssemblyName _refAssembly in _referencedAssemblies)
            {
                if (_refAssembly.Name == "Microsoft.VisualBasic")
                    return true;
            }

            return false;
        }

        // Recursively apply localized resources to a component and its constituent components
        protected virtual void ApplyResources(Type componentType, IComponent instance, CultureInfo culture)
        {
            // check whether there are localizable resources for the type - if not we are done
            Stream _resourceStream =
                componentType.Assembly.GetManifestResourceStream(componentType.FullName + ".resources");

            if (_resourceStream == null)
                return;

            // recursively apply the resources localized in the base type
            Type _parentType = componentType.BaseType;

            if (_parentType != null)
            {
                ApplyResources(_parentType, instance, culture);
            }

            // load the resources for this IComponent type into a sorted list
            ComponentResourceManager _resourceManager = new ComponentResourceManager(componentType);
            SortedList<string, object> _resources = new SortedList<string, object>();
            LoadResources(_resourceManager, culture, _resources);

            // build a lookup table of components indexed by resource name
            Dictionary<string, IComponent> _components = new Dictionary<string, IComponent>();

            // build a lookup table of extender providers indexed by type
            Dictionary<Type, IExtenderProvider> _extenderProviders = new Dictionary<Type, IExtenderProvider>();

            bool _isVb = IsVBAssembly(componentType.Assembly);

            _components["$this"] = instance;
            FieldInfo[] _fields =
                componentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo _field in _fields)
            {
                string _fieldName = _field.Name;

                // in VB the field names are prepended with an "underscore" so we need to remove this
                if (_isVb)
                {
                    _fieldName = _fieldName.Substring(1, _fieldName.Length - 1);
                }

                // check whether this field is a localized component of the parent
                string _resourceName = ">>" + _fieldName + ".Name";

                if (_resources.ContainsKey(_resourceName))
                {
                    IComponent _childComponent = _field.GetValue(instance) as IComponent;

                    if (_childComponent != null)
                    {
                        _components[_fieldName] = _childComponent;

                        // apply resources localized in the child component type
                        ApplyResources(_childComponent.GetType(), _childComponent, culture);

                        // if this component is an extender provider then keep track of it
                        if (_childComponent is IExtenderProvider)
                        {
                            _extenderProviders[_childComponent.GetType()] = _childComponent as IExtenderProvider;
                        }
                    }
                }
            }

            // now process the resources 
            foreach (KeyValuePair<string, object> _pair in _resources)
            {
                string _resourceName = _pair.Key;
                object _resourceValue = _pair.Value;
                string[] _resourceNameParts = _resourceName.Split('.');
                string _componentName = _resourceNameParts[0];
                string _propertyName = _resourceNameParts[1];

                if (_componentName.StartsWith(">>"))
                    continue;

                if (m_excludeProperties.Contains(_propertyName))
                    continue;

                IComponent _component = null;

                if (!_components.TryGetValue(_componentName, out _component))
                    continue;

                // some special case handling for control sizes/locations
                Control _control = _component as Control;

                if (_control != null)
                {
                    switch (_propertyName)
                    {
                        case "Size":
                            SetControlSize(_control, (Size) _resourceValue);
                            continue;
                        case "Location":
                            SetControlLocation(_control, (Point) _resourceValue);
                            continue;
                        case "ClientSize":
                            if (_control is Form && PreserveFormSize)
                                continue;

                            break;
                    }
                }

                // use the property descriptor to set the resource value
                PropertyDescriptor _pd = TypeDescriptor.GetProperties(_component).Find(_propertyName, false);

                if (((_pd != null) && !_pd.IsReadOnly) &&
                    ((_resourceValue == null) || _pd.PropertyType.IsInstanceOfType(_resourceValue)))
                {
                    _pd.SetValue(_component, _resourceValue);
                }
                else
                {
                    // there was no property corresponding to the given resource name.  If this is a control
                    // the property may be an extender property so try applying it as an extender resource
                    if (_control != null)
                    {
                        ApplyExtenderResource(_extenderProviders, _control, _propertyName, _resourceValue);
                    }
                }
            }
        }

        #endregion
    }
}