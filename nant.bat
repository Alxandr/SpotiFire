@for %%X in (cl.exe) do @(
	@set FOUND=%%~$PATH:X
)

@if defined FOUND (
	@REM do nothing
) else (
	call "C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\vcvarsall.bat"
)

tools\nant\nant %*