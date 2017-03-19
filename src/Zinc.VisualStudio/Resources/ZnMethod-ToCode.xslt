<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:zn="urn:zinc"
    xmlns:fn="urn:eo-util"
    exclude-result-prefixes="msxsl zn fn">

    <xsl:output method="text" indent="no" />

    <xsl:param name="ToolVersion" />
    <xsl:param name="FileName" />
    <xsl:param name="FullFileName" />
    <xsl:param name="Namespace" />


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ zn:method/
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" zn:method ">
        <xsl:text>using Dapper;
using Platinum.Data;
using System;
using System.Threading.Tasks;

namespace </xsl:text>
        <xsl:value-of select=" $Namespace " />
        <xsl:text>
{
    public partial class </xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Implementation
    {
        /// &lt;summary /&gt;
        public Task&lt;</xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Response&gt; InnerRun( </xsl:text>
        <xsl:value-of select=" $FileName " />
        <xsl:text>Request request )
        {
            throw new NotImplementedException();
        }
    }
}
</xsl:text>
    </xsl:template>

</xsl:stylesheet>
