Các tiêu chí xác thực để cho phép call api
	- xác thực api key: nếu api key không chính xác thì không được sử dụng
	- xác thực token: token chính xác và trường hợp logout thì token sẽ không được sử dụng
	- xác thực quyền người dùng: nếu người dùng đang sử dụng nhưng bị thu hồi quyền truy cập thì không được sử dụng

Notes:
	- với api key thì cần chú ý ở class JwtAuthenticationAttribute và các api không xác thực token ở CommonController, SystemConfigurationController

Merge code:
	- d5ad71f1300f71a5df18803ba0738d4cfee92bd0
	- merge code latest from commit 78210dd9b9e6c6712e001854956093afc42010f2 in branch phase2_lct on Jul 29, 2025

vi
Microsoft.IdentityModel.Abstractions.dll
App_global.asax.dll
App_global.asax.compiled
System.Diagnostics.DiagnosticSource.dll
System.Diagnostics.DiagnosticSource.xml
System.Security.Cryptography.Algorithms.dll
System.Security.Cryptography.Encoding.dll
System.Security.Cryptography.Primitives.dll
System.Security.Cryptography.X509Certificates.dll