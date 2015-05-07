# ccnet.formatedDateTimeLabeller.plugin
Formated DateTime Labeller plugin for CruiseControl.NET


##Description
Formated DateTime Labeller makes possible to label your builds with customized date time format.
It is based on the dateLabeller included with CCNet.
Thanks also to Toby Qin the author of ccnet.datetimelabeller - I've forked and used his project as template for this plugin

##Introduction

Create build label with specific date time format.
Examples: yy.MM.dd.revision, y.M.d.revision.

yearFormat could be y or yy or yyyy
monthFormat could be M or MM
dayFormat could be d or dd
revisionFormat could be any zero's 0 or 000 or 00000 and etc.
revision will be the last quadrant of the version and it will be restarted every day or if you change the one of the formats of the Labeller 


##Installation

Download ccnet.formatedDateTimeLabeller.plugin.dll file  from Release into the CruiseControl.NET Installation folder (e.g. C:\Program Files\CruiseControl.NET\server)
Restart the CruiseControl.NET Service

1. Start -> Run -> services.msc
2. Right-Click on the CruiseControl.NET Service
3. Restart

Modify your ccnet.config file to effectively use the labeller.

##Use

Modify your ccnet.config file, under the <project> node
```xml
	<labeller type="formatedDateTimeLabeller">
		<yearFormat>y</yearFormat>
		<monthFormat>M</monthFormat>
		<dayFormat>d</dayFormat>
	</labeller>
```

