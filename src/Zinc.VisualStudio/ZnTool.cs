using Platinum.VisualStudio;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Zinc.VisualStudio
{
    public class ZnTool : BaseTool
    {
        /// <summary>
        /// Executes tool.
        /// </summary>
        protected override string Execute( ToolGenerateArgs args )
        {
            /*
             * #1. Apply XSLT
             */
            IGenerator xsltTool = new PtXsltTool();
            string outputContent = xsltTool.Generate( args );


            /*
             * #2. Conditionally generate $MethodImpl.cs skeleton file.
             */
            MethodAddImpl( args );


            /*
             * 
             */
            return outputContent;
        }


        /// <summary>
        /// Generates a bare skeleton method implementation class.
        /// </summary>
        /// <param name="args">Generation arguments.</param>
        private static void MethodAddImpl( ToolGenerateArgs args )
        {
            /*
             * 
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "zn", "urn:zinc" );
            manager.AddNamespace( "vs", "http://schemas.microsoft.com/developer/msbuild/2003" );

            XmlDocument doc = new XmlDocument();
            doc.LoadXml( args.Content );


            /*
             * Is method?
             */
            if ( doc.DocumentElement.NamespaceURI != "urn:zinc" || doc.DocumentElement.LocalName != "method" )
                return;


            /*
             * Already has implementation file?
             */
            string impl = Path.Combine(
                    Path.GetDirectoryName( args.FileName ),
                    Path.GetFileNameWithoutExtension( args.FileName ) + "Impl.cs" );

            Console.WriteLine( impl );
            if ( File.Exists( impl ) == true )
                return;


            /*
             * Is it marked as zn:notImplemented?
             */
            XmlElement notImpl = (XmlElement) doc.SelectSingleNode( " /zn:method/zn:notImplemented ", manager );

            if ( notImpl != null )
                return;


            /*
             * Generate Impl.cs
             */
            var xslt = X.LoadXslt( "ZnMethod-ToCode.xslt" );


            /*
             * 
             */
            FileInfo inputFile = new FileInfo( args.FileName );

            Uri fileUri = new Uri( inputFile.FullName );
            Uri directoryUri = new Uri( inputFile.DirectoryName );
            string rawName = inputFile.Name.Substring( 0, inputFile.Name.Length - inputFile.Extension.Length );

            XsltArgumentList xsltArgs = new XsltArgumentList();
            xsltArgs.AddParam( "ToolVersion", "", Assembly.GetExecutingAssembly().GetName( false ).Version.ToString( 4 ) );
            xsltArgs.AddParam( "FileName", "", rawName );
            xsltArgs.AddParam( "FullFileName", "", inputFile.FullName );
            xsltArgs.AddParam( "UriFileName", "", fileUri.AbsoluteUri );
            xsltArgs.AddParam( "UriDirectory", "", directoryUri.AbsoluteUri );
            xsltArgs.AddParam( "Namespace", "", args.Namespace );

            xsltArgs.AddExtensionObject( "urn:eo-util", new XsltExtensionObject() );

            string implContent = X.ToText( xslt, xsltArgs, doc );

            if ( args.WhatIf == false )
            {
                try
                {
                    File.WriteAllText( impl, implContent );
                }
                catch ( Exception ex )
                {
                    throw new ToolException( $"Failed to write skeleton implementation to '{ impl }'.", ex );
                }
            }


            /*
             * Modify the .csproj
             */
            FileInfo file = new FileInfo( args.FileName );

            FileInfo[] projs = file.Directory.Parent.GetFiles( "*.csproj" );

            if ( projs == null || projs.Length == 0 )
                throw new ToolException( "No .csproj found in parent directory from zn:method." );

            if ( projs.Length != 1 )
                throw new ToolException( "More than one .csproj found in parent directory from zn:method." );

            string csproj = projs[ 0 ].FullName;

            XmlDocument projDoc = new XmlDocument();
            projDoc.Load( csproj );


            /*
             * 

    <Compile Include="OneService\MethodOne.cs">
      <AutoGen>True</AutoGen>
      <DesignTime>True</DesignTime>
      <DependentUpon>MethodOne.xml</DependentUpon>
    </Compile>
    <Compile Include="OneService\MethodOneImpl.cs" />

             * 
             */
            string find = file.Directory.Name + "\\" + Path.GetFileNameWithoutExtension( file.Name ) + ".cs";
            string addf = file.Directory.Name + "\\" + Path.GetFileNameWithoutExtension( file.Name ) + "Impl.cs";

            XmlElement findElem = (XmlElement) projDoc.SelectSingleNode( $" /vs:Project//vs:Compile[ @Include = '{ find }' ] ", manager );
            XmlElement addfElem = (XmlElement) projDoc.SelectSingleNode( $" /vs:Project//vs:Compile[ @Include = '{ addf }' ] ", manager );

            if ( findElem != null && addfElem == null )
            {
                addfElem = projDoc.CreateElement( "Compile", "http://schemas.microsoft.com/developer/msbuild/2003" );

                var include = projDoc.CreateAttribute( "Include" );
                include.Value = addf;
                addfElem.Attributes.Append( include );

                findElem.ParentNode.InsertAfter( addfElem, findElem );

                if ( args.WhatIf == false )
                {
                    try
                    {
                        projDoc.Save( csproj );
                    }
                    catch ( Exception ex )
                    {
                        throw new ToolException( "Failed overwriting project file.", ex );
                    }
                }
            }
        }
    }
}
