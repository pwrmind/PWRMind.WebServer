<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body>
        <h2>Services</h2>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>Name</th>
            <th>Type</th>
            <th>Manager</th>
            <th>Service list</th>
          </tr>
          <xsl:for-each select="Services/Service">
            <tr>
              <td>
                <xsl:value-of select="NAME" />
              </td>
              <td>
                <xsl:value-of select="TYPE" />
              </td>
              <td>
                <a href="#"><xsl:value-of select="MANAGER" /></a>
              </td>
              <td>
                <a href="#"><xsl:value-of select="ITSYS_LIST" /></a>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>