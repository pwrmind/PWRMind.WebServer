<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body>
        <h2>Services</h2>
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