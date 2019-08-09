using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyBBS
{
	public static class Resource
	{
		public const string HTML_MAIN = @"
<html>
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
</head>
<body>
BBS
<hr/>

${TIMELINE}

<hr/>
<form method=""POST"" action=""/remark"">
name: <input type=""text"" name=""user"" value=""${USER}""/>
e-mail address: <input type=""text"" name=""e-mail"" value=""${E-MAIL}""/>

<br/>

<textarea
	name=""message""
	rows=""5""
	cols=""100""
	maxlength=""500""
	>
</textarea>

<br/>

<input type=""submit"" value=""Send"" />
<input type=""reset"" value=""Clear"" />

</form>

<script>

window.onload = function() {
	if(location.pathname == ""/return"") {
		window.scrollTo(0,document.body.scrollHeight);
	}
};

</script>

</body>
</html>
";

		public const string HTML_REMARK = @"
${DATE-TIME}　${IP}　${USER}　${E-MAIL}
<br/>
${MESSAGE}
<br/>
<br/>
";

		public const string HTML_REMARKED = @"
<html>
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
<meta http-equiv=""refresh"" content=""1;URL='/return?user=${USER}&e-mail=${E-MAIL}"" />
</head>
<body>

<a href=""/return?user=${USER}&e-mail=${E-MAIL}"">return to BBS</a>

</body>
</html>
";
	}
}
