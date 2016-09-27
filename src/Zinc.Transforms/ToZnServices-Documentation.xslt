<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:zn="urn:zinc"
    xmlns:err="urn:platinum/actor"
    xmlns:rest="urn:zinc/rest"
    xmlns:eo="urn:eo-util"
    exclude-result-prefixes="msxsl zn err rest eo">

    <xsl:param name="FileName" />
    <xsl:param name="Namespace" />
    <xsl:param name="UriDirectory" />

    <xsl:output method="html" indent="yes" />

    <xsl:template match=" zn:services ">
        <html>
            <head>
                <title>Service documentation</title>
            </head>
            <body>
                <style>
                    <xsl:text>

body {
    font-family: Arial, sans-serif;
    margin: 15px;
}

h1 {
    margin-top: 30px;
    margin-bottom: 0;
    background-color: #FFF000;
    margin-left: -15px;
    margin-right: -15px;
    padding-left: 15px;
    padding-top: 10px;
    padding-bottom: 5px;
}

h2 {
    font-family: Consolas, Courier New, monospace;
    background-color: #FFFBC9;
    margin-top: 0;
    margin-bottom: 0;
    margin-left: -15px;
    margin-right: -15px;
    padding-top: 10px;
    padding-bottom: 5px;
    padding-left: 15px;
}

.summary {
    padding: 5px;
}

.method {
    margin-bottom: 20px;
}

table {
    border-collapse: collapse;
    table-layout: fixed;
    width: 100%;
}

td {
    padding: 3px 5px;
    vertical-align: top;
}

.req-resp td {
    padding-top: 7px;
    font-weight: bold;
    background-color: #EEE;
    border-bottom: 1px solid #999;
}

.no-inout td {
    font-style: italic;
}

td.ev {
    background-color: #F5F5F5;
}


td.od {
    background-color: white;
}

.fixed {
    font-family: Consolas, Courier New, monospace;
    font-size: 90%;
}

</xsl:text>
                </style>

                <xsl:apply-templates select=" zn:service " />
            </body>
        </html>
    </xsl:template>

    <xsl:template match=" zn:service ">
        <h1>
            <xsl:value-of select=" @name " />
            <xsl:text>Services</xsl:text>
        </h1>

        <xsl:apply-templates select=" zn:method ">
            <xsl:sort select=" @name " />
        </xsl:apply-templates>
    </xsl:template>

    <xsl:template match=" zn:method ">
        <xsl:variable name="file" select=" concat( ../@name, 'Service/', @name, '.xml' ) " />
        <xsl:variable name="root" select=" document( $file ) " />
        <xsl:variable name="errors" select=" document( 'Errors.xml' )/err:errors " />

        <xsl:apply-templates select=" $root/zn:method " mode="zn:method">
            <xsl:with-param name="name" select=" @name " />
            <xsl:with-param name="errors" select=" $errors " />
        </xsl:apply-templates>
    </xsl:template>


    <xsl:template match=" zn:method " mode="zn:method">
        <xsl:param name="name" />
        <xsl:param name="errors" />

        <xsl:variable name="span" select=" '3' " />

        <div class="method">
            <h2>
                <xsl:value-of select=" $name " />
            </h2>

            <div class="summary">
                <xsl:value-of select=" zn:summary " />
            </div>

            <table>
                <colgroup>
                    <col width="200px" />
                    <col width="100px" />
                    <col />
                </colgroup>

                <tbody>
                    <tr class="req-resp">
                        <td colspan="{ $span }">Request</td>
                    </tr>

                    <xsl:choose>
                        <xsl:when test=" zn:request/* ">
                            <xsl:apply-templates select=" zn:request/* " mode="prop">
                                <xsl:with-param name="indent" select=" 0 " />
                            </xsl:apply-templates>
                        </xsl:when>
                        <xsl:otherwise>
                            <tr class="no-inout">
                                <td colspan="{ $span }">No request parameters.</td>
                            </tr>
                        </xsl:otherwise>
                    </xsl:choose>

                    <tr class="req-resp">
                        <td colspan="{ $span }">Response</td>
                    </tr>

                    <xsl:choose>
                        <xsl:when test=" zn:response/* ">
                            <xsl:apply-templates select=" zn:response/* " mode="prop">
                                <xsl:with-param name="indent" select=" 0 " />
                            </xsl:apply-templates>
                        </xsl:when>
                        <xsl:otherwise>
                            <tr class="no-inout">
                                <td colspan="{ $span }">No response parameters.</td>
                            </tr>
                        </xsl:otherwise>
                    </xsl:choose>

                    <xsl:if test=" $errors/err:error[ starts-with( @id, $name ) ] ">
                        <tr class="req-resp">
                            <td colspan="{ $span }">Errors</td>
                        </tr>

                        <xsl:apply-templates select=" $errors/err:error[ starts-with( @id, $name ) ] ">
                            <xsl:with-param name="name" select=" $name " />
                        </xsl:apply-templates>
                    </xsl:if>
                </tbody>
            </table>
        </div>
    </xsl:template>


    <xsl:template match=" err:error ">
        <xsl:param name="name" />

        <xsl:variable name="class">
            <xsl:choose>
                <xsl:when test=" position() mod 2 = 0 ">
                    <xsl:text>ev</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>od</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>

        <tr>
            <td class="{ $class } fixed" colspan="2">
                <xsl:value-of select=" substring-after( @id, $name ) " />
                <br />
                <xsl:value-of select=" @code " />
            </td>
            <td class="{ $class }">
                <xsl:value-of select=" err:description " />
            </td>
        </tr>
    </xsl:template>


    <xsl:template match=" * " mode="prop">
        <xsl:param name="indent" />
        <xsl:param name="parentClass" select=" '' " />

        <xsl:variable name="class">
            <xsl:choose>
                <xsl:when test=" position() mod 2 = 0 ">
                    <xsl:text>ev</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>od</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:variable>

        <tr>
            <td class="{ $class } fixed">
                <xsl:value-of select=" @name " />
            </td>
            <td class="{ $class } fixed">
                <xsl:choose>
                    <xsl:when test=" @type ">
                        <xsl:value-of select=" @type " />
                    </xsl:when>

                    <xsl:otherwise>
                        <xsl:value-of select=" local-name(.) "/>
                    </xsl:otherwise>
                </xsl:choose>

                <xsl:choose>
                    <xsl:when test=" @array = 'true' ">
                        <xsl:text>[]</xsl:text>
                    </xsl:when>
                    <xsl:when test=" @optional = 'true' ">
                        <xsl:text>?</xsl:text>
                    </xsl:when>
                </xsl:choose>
            </td>
            <td class="{ $class } ">
                <xsl:value-of select=" zn:summary " />
            </td>
        </tr>

        <xsl:if test=" local-name(.) = 'structure' ">
            <tr>
                <td class="{ $class }" colspan="3" style=" padding-left: { ( $indent+1) * 20 }px">
                    <table>
                        <colgroup>
                            <col width="200px" />
                            <col width="100px" />
                            <col />
                        </colgroup>
                        <xsl:apply-templates select=" *[ local-name(.) != 'summary' ] " mode="prop">
                            <xsl:with-param name="indent" select=" $indent + 1 " />
                        </xsl:apply-templates>
                    </table>
                </td>
            </tr>
        </xsl:if>
    </xsl:template>


</xsl:stylesheet>
