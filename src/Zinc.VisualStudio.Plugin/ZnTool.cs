using Microsoft.VisualStudio.Shell;
using Platinum.VisualStudio.Plugin;
using System;
using System.Runtime.InteropServices;
using VSLangProj80;

namespace Zinc.VisualStudio.Plugin
{
    /// <summary>
    /// Generate code-behind for Services / Methods. For Methods, also adds
    /// initial (empty) implementation for Method -- adding file to .csproj!
    /// </summary>
    [ComVisible( true )]
    [Guid( "f2b83cff-e664-472b-800d-09ad50d00a82" )]
    [CodeGeneratorRegistration( typeof( ZnTool ), "Zn Tool", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true )]
    [ProvideObject( typeof( ZnTool ) )]
    public class ZnTool : BasePlugin<Zinc.VisualStudio.ZnTool>
    {
    }
}
