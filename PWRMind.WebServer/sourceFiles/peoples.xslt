<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body>
        <h2>Services</h2>
        <table border="1">
          <tr bgcolor="#9acd32">
            <th>Number</th>
            <th>Full name</th>
          </tr>
          <xsl:for-each select="Peoples/People">
            <tr>
              <td>
                <xsl:value-of select="EMPLOYEE_NUMBER" />
              </td>
              <td>
                <a href="#"><span><xsl:value-of select="LAST_NAME" /> <xsl:value-of select="FIRST_NAME" /> <xsl:value-of select="MIDDLE_NAMES" /></span></a>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>