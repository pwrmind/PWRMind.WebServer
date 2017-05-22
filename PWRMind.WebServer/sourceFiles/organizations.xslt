<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="ORDERBY"></xsl:param>
  <xsl:param name="FILTER"></xsl:param>
  <xsl:template match="/">
    <html>
      <body>
        <h2>Services</h2>
        <p>
          Count: <xsl:value-of select="count(/Organizations/Organization)"/>
        </p>
        <p>
          Order by: <xsl:value-of select="$ORDERBY" />
        </p>
        <p>
          Filter by: <xsl:value-of select="$FILTER" />
        </p>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>Id</th>
            <th>Name</th>
          </tr>
          <xsl:for-each select="Organizations/Organization">
            <tr>
              <td>
                <xsl:value-of select="ORG_ID" />
              </td>
              <td>
                <xsl:value-of select="ORG_NAME" />
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>