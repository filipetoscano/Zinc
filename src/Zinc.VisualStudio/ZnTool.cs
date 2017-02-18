using Microsoft.VisualStudio.Shell;
using Platinum.VisualStudio;
using System;
using System.Runtime.InteropServices;
using VSLangProj80;

namespace Zinc.VisualStudio
{
    /// <summary>
    /// Generate configuration section handler.
    /// </summary>
    [ComVisible( true )]
    [Guid( "f2b83cff-e664-472b-800d-09ad50d00a82" )]
    [CodeGeneratorRegistration( typeof( ZnTool ), "Test Tool", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true )]
    [ProvideObject( typeof( ZnTool ) )]
    public class ZnTool : BaseTool
    {
        /// <summary>
        /// Executes tool.
        /// </summary>
        protected override string Execute( string inputNamespace, string inputFileName, string inputContent, bool whatIf )
        {
            #region Validations

            if ( inputNamespace == null )
                throw new ArgumentNullException( nameof( inputNamespace ) );

            if ( inputFileName == null )
                throw new ArgumentNullException( nameof( inputFileName ) );

            if ( inputContent == null )
                throw new ArgumentNullException( nameof( inputContent ) );

            #endregion

            return "// ZnTool";
        }
    }
}
