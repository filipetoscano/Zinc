using Microsoft.VisualStudio.Shell;
using Platinum.VisualStudio;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using VSLangProj80;

namespace Zinc.VisualStudio
{
    /// <summary>
    /// Generate code-behind for Services / Methods. For Methods, also adds
    /// initial (empty) implementation for Method -- adding file to .csproj!
    /// </summary>
    [ComVisible( true )]
    [Guid( "f2b83cff-e664-472b-800d-09ad50d00a82" )]
    [CodeGeneratorRegistration( typeof( ZnTool ), "Zn Tool", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true )]
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


            /*
             * 
             */
            ToolGenerateArgs args = new ToolGenerateArgs()
            {
                Content = inputContent,
                FileName = inputFileName,
                Namespace = inputNamespace,
                WhatIf = whatIf,
            };

            IToolChain xsltTool = new PtXsltTool();
            string outputContent = xsltTool.Generate( args );


            /*
             * 
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "zn", "urn:zinc" );

            XmlDocument doc = new XmlDocument();
            doc.LoadXml( inputContent );

            if ( doc.DocumentElement.NamespaceURI == "urn:zinc" 
                 && doc.DocumentElement.LocalName == "method" )
            {
                string impl = Path.GetFileNameWithoutExtension( inputFileName ) + "Impl.cs";

                if ( File.Exists( impl ) == false )
                {
                    XmlElement notImpl = (XmlElement) doc.SelectSingleNode( " /zn:method/zn:notImplemented " );

                    if ( notImpl == null )
                    {
                        GenerateAddImpl( args );
                    }
                }
            }


            /*
             * 
             */
            return outputContent;
        }


        private void GenerateAddImpl( ToolGenerateArgs args )
        {
            /*

    <Compile Include="OneService\MethodOne.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>MethodOne.xml</DependentUpon>
    </Compile>
    <Compile Include="OneService\MethodOneImpl.cs" />

             */


            /*
             * 
             */
            FileInfo file = new FileInfo( args.FileName );

            FileInfo[] projs = file.Directory.Parent.GetFiles( "*.csproj" );

            if ( projs == null || projs.Length == 0 )
                throw new ToolException( "No .csproj found in parent directory from zn:method." );

            if ( projs.Length != 1 )
                throw new ToolException( "More than one .csproj found in parent directory from zn:method." );

            string csproj = projs[ 0 ].FullName;
            Console.WriteLine( csproj );


            /*
             * 
             */
            string find = file.DirectoryName + "\\" + Path.GetFileNameWithoutExtension( file.Name ) + ".cs";
            string addf = file.DirectoryName + "\\" + Path.GetFileNameWithoutExtension( file.Name ) + "Impl.cs";

            Console.WriteLine( find );
            Console.WriteLine( addf );
        }
    }
}
