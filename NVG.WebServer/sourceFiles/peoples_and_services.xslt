<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
		<head>
			
			<style>
				table, tr, td {
					border: 1px dotted red;
				}
			</style>
		</head>
      <body>
        <h2>Peoples and services</h2>
        <table>
          <tr bgcolor="#9acd32">
            <th>Number</th>
            <th>Full name</th>
          </tr>
          <xsl:for-each select="Data/Peoples/People">
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
		<table>
          <tr bgcolor="#9acd32">
            <th>Name</th>
            <th>Type</th>
            <th>Manager</th>
            <th>Service list</th>
          </tr>
          <xsl:for-each select="Data/Services/Service">
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
                <xsl:value-of select="ITSYS_LIST" />
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>