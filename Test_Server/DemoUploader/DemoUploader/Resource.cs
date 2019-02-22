using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoUploader
{
	public class Resource
	{
		public const string HTML_MAIN = @"
<html>
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
</head>
<body>
Uploader Demo
<hr/>

<form method=""POST"" action=""/upload"" enctype=""multipart/form-data"">

file: <input type=""file"" name=""upload-file""/>
<hr/>
supplement: <input type=""text"" name=""supplement"" value=""ここにコメントを書いて下さい。"" style=""width: 80%;""/>
<hr/>

<input type=""submit"" value=""Upload"" />
<input type=""reset"" value=""Clear"" />

</form>

<fieldset>
<legend>Notice</legend>
大きなファイルをアップロードしようとすると、HTT_RPC の制限によって失敗するかもしれません。<br/>
タスクトレイの HTT_RPC アイコンを　右クリック　⇒　設定　⇒　詳細設定　から Request Content-Length Max を十分大きな値にして下さい。<br/>
アップロードするファイルのサイズより少し大きい値を設定して下さい。例えば 100,000,000 バイトのファイルをアップロード可能にしたい場合は 101,000,000 くらいを設定して下さい。<br/>
</fieldset>

</body>
</html>
";

		public const string HTML_UPLOADED = @"
<html>
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""/>
</head>
<body>
uploaded file:<br/>

<_overview>
	<_image>
		<a href=""/uploaded-file/${FILE-NAME}"">
			<img src=""/uploaded-file/${FILE-NAME}"" style=""max-height: 75vh;"" />
		</a>
	</_image>
	<_video>
		<video src=""/uploaded-file/${FILE-NAME}"" controls style=""max-height: 75vh;""></video>
	</_video>
	<_audio>
		<audio src=""/uploaded-file/${FILE-NAME}"" controls></audio>
	</_audio>
	<_default>
		<a href=""/uploaded-file/${FILE-NAME}"">Open</a>&nbsp;
		<a href=""/uploaded-file/${FILE-NAME}"" download>Download</a>
	</_default>
</_overview>

<hr/>
file-name: ${FILE-NAME}<br/>
file-size: ${FILE-SIZE} byte(s)<br/>
supplement: ${SUPPLEMENT}<br/>
<hr/>
uploaded file data(hex):<br/>

<div style=""word-wrap: break-word;"">
${FILE-DATA-HEX}
</div>

<hr/>

<a href=""/"">Return to home</a>

</body>
</html>
";
	}
}
