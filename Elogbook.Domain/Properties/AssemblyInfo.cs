using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Elogbook.Domain")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("HP Inc.")]
[assembly: AssemblyProduct("Elogbook.Domain")]
[assembly: AssemblyCopyright("Copyright © HP Inc. 2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("acea7b0b-2054-4645-8afc-01a9f1780ed0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
{

    private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

    public static Settings Default
    {
        get
        {
            return defaultInstance;
        }
    }

    [global::System.Configuration.ApplicationScopedSettingAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
    [global::System.Configuration.DefaultSettingValueAttribute("http://171.244.149.211:8080/apws.asmx")]
    public string DEMO_FULL_CA_DOTNET_WEB_MobileCAWS_APWsImplService
    {
        get
        {
            return ((string)(this["DEMO_FULL_CA_DOTNET_WEB_MobileCAWS_APWsImplService"]));
        }
    }
}